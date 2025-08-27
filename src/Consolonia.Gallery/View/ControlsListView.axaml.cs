using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Gallery.Gallery;
using Consolonia.Themes;

namespace Consolonia.Gallery.View
{
    public enum ThemesList
    {
        Modern,
        TurboVision
    }

    public partial class ControlsListView : Window
    {
        private static readonly HttpClient Client = new();
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
            string[] commandLineArgs = _commandLineArgs.Where(s => s != null)
                .Where(s => !s.ToUpper().EndsWith(App.TurboVisionProgramParameterUpperCase)).ToArray();
            if (commandLineArgs.Length == 0)
            {
                GalleryGrid.SelectedIndex = 0;
                return;
            }

            string itemToSelectName = commandLineArgs.Last();
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
            string xamlFile = $"{selectedItem.Type.Name}.axaml";
            await ShowCode(xamlFile);
        }

        private async void OnShowCodeBehind(object sender, RoutedEventArgs e)
        {
            var selectedItem = GalleryGrid.SelectedItem as GalleryItem;
            string xamlFile = $"{selectedItem.Type.Name}.axaml.cs";
            await ShowCode(xamlFile);
        }

        private static async Task ShowCode(string xamlFile)
        {
            string xaml = await Client.GetStringAsync(new Uri(
                $"https://raw.githubusercontent.com/jinek/Consolonia/refs/heads/main/src/Consolonia.Gallery/Gallery/GalleryViews/{xamlFile}"));

            var dialog = new XamlDialogWindow
            {
                Title = xamlFile,
                // ReSharper disable once MethodHasAsyncOverload
                DataContext = xaml
            };
            await dialog.ShowDialog();
        }


        private void OnThemeMenuItemClick(object? sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem { Tag: string themeName } ||
                !Enum.TryParse(themeName, out ThemesList selectedTheme))
                return;

            Application.Current.Styles[0] = selectedTheme switch
            {
                ThemesList.Modern => new ModernTheme(),
                ThemesList.TurboVision => new TurboVisionTheme(),
                _ => throw new InvalidDataException("Unknown theme name")
            };

            UpdateThemeMenuItems();
        }

        private void OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            UpdateThemeMenuItems();
        }

        private void UpdateThemeMenuItems()
        {
            string themeName = Application.Current.Styles[0].GetType().Name[..^5];
            ThemeModernMenuItem.IsChecked = themeName == nameof(ThemesList.Modern);
            ThemeTurboVisionMenuItem.IsChecked = themeName == nameof(ThemesList.TurboVision);
        }
    }

    public partial class ControlsListViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsTurboVision))]
        [NotifyPropertyChangedFor(nameof(IsModern))]
        private string _selectedTheme;

        public bool IsModern => SelectedTheme == nameof(ThemesList.Modern);
        public bool IsTurboVision => SelectedTheme == nameof(ThemesList.TurboVision);
    }
}