using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ControlCatalog.ViewModels;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(70)]
    public class GalleryListBox : UserControl
    {
        public GalleryListBox()
        {
            InitializeComponent();
            DataContext = new ListBoxPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}