using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Gallery.Gallery;
using Consolonia.Themes.TurboVision.Templates;
using Consolonia.Themes.TurboVision.Themes.Fluent;
using Consolonia.Themes.TurboVision.Themes.Material;
using Consolonia.Themes.TurboVision.Themes.TurboVisionBlack;
using Consolonia.Themes.TurboVision.Themes.TurboVisionDark;

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

        private void MaterialTheme_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Model.SelectedTheme = Themes.Material;
            this.Styles[0] = new MaterialTheme();
        }

        private void FluentTheme_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Model.SelectedTheme = Themes.Fluent;
            this.Styles[0] = new FluentTheme();
        }

        private void TurboVisionTheme_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Model.SelectedTheme = Themes.TurboVision;
            this.Styles[0] = new TurboVisionTheme();
        }

        private void TurboVisionDarkTheme_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Model.SelectedTheme = Themes.TurboVisionDark;
            this.Styles[0] = new TurboVisionDarkTheme();
        }

        private void TurboVisionBlackTheme_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Model.SelectedTheme = Themes.TurboVisionBlack;
            this.Styles[0] = new TurboVisionBlackTheme();
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
        private Themes _selectedTheme;

        public bool IsMaterial => SelectedTheme == Themes.Material;
        public bool IsFluent => SelectedTheme == Themes.Fluent;
        public bool IsTurboVision => SelectedTheme == Themes.TurboVision;
        public bool IsTurboVisionDark => SelectedTheme == Themes.TurboVisionDark;
        public bool IsTurboVisionBlack => SelectedTheme == Themes.TurboVisionBlack;
    }
}