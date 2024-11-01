using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(30)]
    public partial class GalleryCanvas : UserControl
    {
        public GalleryCanvas()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}