using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Gallery.Gallery;
using Consolonia.Themes;

namespace Consolonia.Gallery.View
{
    public enum ThemesList
    {
        Material,
        Fluent,
        TurboVision,
        TurboVisionDark,
        TurboVisionBlack
    }

    public partial class ControlsListView : DockPanel
    {
        private readonly IEnumerable<GalleryItem> _items;
        private string[] _commandLineArgs;

        public ControlsListView()
        {
            InitializeComponent();

            DataContext = new ControlsListViewModel();

            GalleryGrid.ItemsSource = _items = GalleryItem.Enumerated.ToArray();

            if (Application.Current!.ApplicationLifetime is ConsoloniaLifetime lifetime)
                _commandLineArgs = lifetime!.Args!;
            else
                _commandLineArgs = [];

            TrySetupSelected();

            Loaded += OnLoaded;
        }

        private void TrySetupSelected()
        {
            if (_commandLineArgs.Length is not 1 and not 2)
            {
                GalleryGrid.SelectedIndex = 0;
                return;
            }

            string itemToSelectName = _commandLineArgs.Last();
            GalleryItem itemToSelect;
            try
            {
                itemToSelect = _items.SingleOrDefault(item =>
                    string.Equals(item.Name, itemToSelectName, StringComparison.OrdinalIgnoreCase));
                if (itemToSelect == null)
                    throw new ArgumentOutOfRangeException(
                        $"No item with name {itemToSelectName} found. List of possible item names: {string.Join(", ", GalleryItem.Enumerated.Select(item => item.Name))}");
            }
            catch (InvalidOperationException)
            {
                throw new InvalidProgramException(
                    $"Several gallery items found with provided name {itemToSelectName}");
            }

            GalleryGrid.SelectedItem = itemToSelect;
            GalleryGrid.Focus();
        }


        public void ChangeTo(string[] args)
        {
            _commandLineArgs = args;
            TrySetupSelected();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var lifetime = Application.Current!.ApplicationLifetime as ConsoloniaLifetime;
            lifetime.TryShutdown();
        }

        private async void OnShowXaml(object sender, RoutedEventArgs e)
        {
            var selectedItem = GalleryGrid.SelectedItem as GalleryItem;
            var xamlFile = $"{selectedItem.Type.Name}.axaml";
            await ShowCode(xamlFile);
        }
        private async void OnShowCodeBehind(object sender, RoutedEventArgs e)
        {
            var selectedItem = GalleryGrid.SelectedItem as GalleryItem;
            var xamlFile = $"{selectedItem.Type.Name}.axaml.cs";
            await ShowCode(xamlFile);
        }

        private static readonly HttpClient Client = new HttpClient();

        private static async Task ShowCode(string xamlFile)
        {
            var lifetime = Application.Current.ApplicationLifetime as ISingleViewApplicationLifetime;
            if (lifetime == null)
                throw new InvalidOperationException("ApplicationLifetime is not ISingleViewApplicationLifetime");

            string xaml = await Client.GetStringAsync(new Uri($"https://raw.githubusercontent.com/jinek/Consolonia/refs/heads/main/src/Consolonia.Gallery/Gallery/GalleryViews/{xamlFile}"));

            var dialog = new XamlDialogWindow
            {
                Title = xamlFile,
                // ReSharper disable once MethodHasAsyncOverload
                DataContext = xaml
            };
            await dialog.ShowDialog();
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeCombo?.SelectedItem is not ComboBoxItem selectedItem ||
                selectedItem.Content is not string themeName ||
                !Enum.TryParse(themeName, out ThemesList selectedTheme))
                return;

            Application.Current.Styles[0] = selectedTheme switch
            {
                ThemesList.Material => new MaterialTheme(),
                ThemesList.Fluent => new FluentTheme(),
                ThemesList.TurboVision => new TurboVisionTheme(),
                ThemesList.TurboVisionDark => new TurboVisionDarkTheme(),
                ThemesList.TurboVisionBlack => new TurboVisionBlackTheme(),
                _ => throw new InvalidDataException("Unknown theme name")
            };
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            string themeName = Application.Current.Styles[0].GetType().Name[..^5];
            ThemeCombo.SelectedIndex = (int)Enum.Parse<ThemesList>(themeName);
        }
    }

    public partial class ControlsListViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTurboVisionDark))]
        [NotifyPropertyChangedFor(nameof(IsTurboVisionBlack))]
        [NotifyPropertyChangedFor(nameof(IsTurboVision))]
        [NotifyPropertyChangedFor(nameof(IsFluent))]
        [NotifyPropertyChangedFor(nameof(IsMaterial))]
        private string _selectedTheme;

        public bool IsMaterial => SelectedTheme == nameof(ThemesList.Material);
        public bool IsFluent => SelectedTheme == nameof(ThemesList.Fluent);
        public bool IsTurboVision => SelectedTheme == nameof(ThemesList.TurboVision);
        public bool IsTurboVisionDark => SelectedTheme == nameof(ThemesList.TurboVisionDark);
        public bool IsTurboVisionBlack => SelectedTheme == nameof(ThemesList.TurboVisionBlack);
    }
}