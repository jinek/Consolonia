using Avalonia.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(70)]
    public partial class GalleryListBox : UserControl
    {
        public GalleryListBox()
        {
            InitializeComponent();
            DataContext = new ListBoxPageViewModel();
        }

    }
}