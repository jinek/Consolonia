using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(40)]
    public partial class GalleryCheckBox : UserControl
    {
        public GalleryCheckBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}