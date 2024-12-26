using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Gallery.Gallery;
using Consolonia.Themes;

namespace Consolonia.Gallery.View
{
    public enum Themes
    {
        Material,
        Fluent,
        TurboVision,
        TurboVisionDark,
        TurboVisionBlack
    }

    public partial class ControlsListView : Window
    {
        private readonly IEnumerable<GalleryItem> _items;
        private string[] _commandLineArgs;

        public ControlsListView()
        {
            InitializeComponent();

            DataContext = new ControlsListViewModel();

#if DEBUG
            this.AttachDevTools();
#endif
            GalleryGrid.ItemsSource = _items = GalleryItem.Enumerated.ToArray();

            if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
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
                    string.Equals(item.Name, itemToSelectName, StringComparison.CurrentCultureIgnoreCase));
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
            Close();
        }

        private async void OnShowXaml(object sender, RoutedEventArgs e)
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;

            var selectedItem = GalleryGrid.SelectedItem as GalleryItem;
            string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "..", "..", "Gallery",
                "GalleryViews", $"{selectedItem.Type.Name}.axaml"));
            var dialog = new XamlDialogWindow
            {
                // ReSharper disable once MethodHasAsyncOverload
                DataContext = File.ReadAllText(path)
            };

            await dialog.ShowDialogAsync(lifetime.MainWindow);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeCombo?.SelectedItem is not ComboBoxItem selectedItem ||
                selectedItem.Content is not string themeName ||
                !Enum.TryParse(themeName, out Themes selectedTheme))
                return;

            Application.Current.Styles[0] = selectedTheme switch
            {
                Themes.Material => new MaterialTheme(),
                Themes.Fluent => new FluentTheme(),
                Themes.TurboVision => new TurboVisionTheme(),
                Themes.TurboVisionDark => new TurboVisionDarkTheme(),
                Themes.TurboVisionBlack => new TurboVisionBlackTheme(),
                _ => throw new ArgumentOutOfRangeException(nameof(selectedTheme))
            };
        }
        
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            string themeName = Application.Current.Styles[0].GetType().Name[..^5];
            ThemeCombo.SelectedIndex = (int)Enum.Parse<Themes>(themeName);
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

        public bool IsMaterial => SelectedTheme == nameof(Themes.Material);
        public bool IsFluent => SelectedTheme == nameof(Themes.Fluent);
        public bool IsTurboVision => SelectedTheme == nameof(Themes.TurboVision);
        public bool IsTurboVisionDark => SelectedTheme == nameof(Themes.TurboVisionDark);
        public bool IsTurboVisionBlack => SelectedTheme == nameof(Themes.TurboVisionBlack);
    }
}