using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    public class GalleryRadioButton : UserControl
    {
        public GalleryRadioButton()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}