using System.Collections.Generic;
using System.Windows.Input;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class MenuItemViewModel
    {
        public string Header { get; init; }
        public ICommand Command { get; init; }
        public object CommandParameter { get; init; }
        public IList<MenuItemViewModel> Items { get; init; }
    }
}