using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Consolonia.Gallery.Gallery;

namespace Consolonia.Gallery.View
{
    public partial class ControlsListView : Window
    {
        private readonly DataGrid _grid;
        private string[] _commandLineArgs;
        private readonly IEnumerable<GalleryItem> _items;

        public ControlsListView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            _grid = this.FindControl<DataGrid>("Grid");
            
            _grid.ItemsSource = _items = GalleryItem.Enumerated.ToArray();

            var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
                _commandLineArgs = lifetime!.Args!;
            else
                _commandLineArgs = Array.Empty<string>();

            TrySetupSelected();
        }

        private void TrySetupSelected()
        {
            if (_commandLineArgs.Length is not 1 and not 2)
            {
                _grid.SelectedIndex = 0;
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

            _grid.SelectedItem = itemToSelect;
            _grid.Focus();
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
    }
}