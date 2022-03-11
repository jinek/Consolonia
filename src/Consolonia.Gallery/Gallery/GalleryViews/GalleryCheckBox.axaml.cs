using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    [GalleryOrder(40)]
    public class GalleryCheckBox : UserControl
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