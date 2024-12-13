using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
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
        private string[] _commandLineArgs;
        private readonly IEnumerable<GalleryItem> _items;

        public ControlsListView()
        {
            InitializeComponent();

            this.DataContext = new ControlsListViewModel();

#if DEBUG
            this.AttachDevTools();
#endif
            this.Grid.ItemsSource = _items = GalleryItem.Enumerated.ToArray();

            var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
                _commandLineArgs = lifetime!.Args!;
            else
                _commandLineArgs = Array.Empty<string>();

            this.Styles.Add(new MaterialTheme());

            TrySetupSelected();
        }

        public ControlsListViewModel Model => (ControlsListViewModel)DataContext;

        private void TrySetupSelected()
        {
            if (_commandLineArgs.Length is not 1 and not 2)
            {
                this.Grid.SelectedIndex = 0;
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

            this.Grid.SelectedItem = itemToSelect;
            this.Grid.Focus();
        }


        public void ChangeTo(string[] args)
        {
            _commandLineArgs = args;
            TrySetupSelected();
        }

        private void Exit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, Avalonia.Controls.SelectionChangedEventArgs e)
        {
            if (ThemeCombo?.SelectedItem is not ComboBoxItem selectedItem ||
                selectedItem.Content is not string themeName ||
                !Enum.TryParse<Themes>(themeName, out var selectedTheme))
            {
                return;
            }

            Styles[0] = selectedTheme switch
            {
                Themes.Material => new MaterialTheme(),
                Themes.Fluent => new FluentTheme(),
                Themes.TurboVision => new TurboVisionTheme(),
                Themes.TurboVisionDark => new TurboVisionDarkTheme(),
                Themes.TurboVisionBlack => new TurboVisionBlackTheme(),
                _ => throw new ArgumentOutOfRangeException(nameof(selectedTheme))
            };
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