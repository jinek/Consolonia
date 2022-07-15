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
    public class ControlsListView : Window
    {
        public ControlsListView()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            var grid = this.FindControl<DataGrid>("Grid");

            IEnumerable<GalleryItem> items;
            grid.Items = items = GalleryItem.Enumerated.ToArray();

            TrySetupSelected();

            void TrySetupSelected()
            {
                string[] commandLineArgs =
                    ((ClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime)!.Args!;

                if (commandLineArgs.Length is not 1 or 2)
                {
                    grid.SelectedIndex = 0;
                    return;
                }

                string itemToSelectName = commandLineArgs.Last();
                GalleryItem itemToSelect;
                try
                {
                    itemToSelect = items.SingleOrDefault(item =>
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

                grid.SelectedItem = itemToSelect;
                grid.Focus();
            }
        }


        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}