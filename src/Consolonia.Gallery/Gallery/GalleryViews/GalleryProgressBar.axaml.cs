using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryProgressBar : UserControl
    {
        public GalleryProgressBar()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}