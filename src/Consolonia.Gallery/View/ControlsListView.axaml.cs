using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Consolonia.Gallery.Gallery;

namespace Consolonia.Gallery.View
{
    public class ControlsListView : Window
    {
        private readonly IEnumerable<GalleryItem> _items;


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public ControlsListView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var grid = this.FindControl<DataGrid>("grid");

            grid.Items = _items = GalleryItem.Enumerated.ToArray();
            
            TrySetupSelected();

            void TrySetupSelected()
            {
                string[] commandLineArgs = Environment.GetCommandLineArgs();

                if (commandLineArgs.Length != 2)
                {
                    grid.SelectedIndex = 0;
                    return;
                }
                string itemToSelectName = commandLineArgs[1];
                GalleryItem? itemToSelect;
                try
                {
                    itemToSelect = _items.SingleOrDefault(item =>
                        string.Equals(item.Name, itemToSelectName, StringComparison.CurrentCultureIgnoreCase));
                    if (itemToSelect == null)
                    {
                        throw new ArgumentOutOfRangeException(
                            $"No item with name {itemToSelectName} found. List of possible item names: {string.Join(", ", GalleryItem.Enumerated.Select(item => item.Name))}");
                    }
                }
                catch (InvalidOperationException)
                {
                    throw new InvalidProgramException(
                        $"Several gallery items found with provided name {itemToSelectName}");
                }

                grid.SelectedItem = itemToSelect;
                grid.Focus();
            }
        }
    }
}