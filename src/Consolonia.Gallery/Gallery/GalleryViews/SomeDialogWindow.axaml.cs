using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class SomeDialogWindow : UserControl
    {
        public SomeDialogWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}