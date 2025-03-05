using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Iciclecreek.Avalonia.WindowManager;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryWindows : UserControl
    {
        public GalleryWindows()
        {
            InitializeComponent();
        }

        private void OnShowWindow(object? sender, RoutedEventArgs e)
        {
            this.ShowWindow(new MyWindow());
        }

        private void OnShowChildWindow(object? sender, RoutedEventArgs e)
        {
            this.ChildWindowManager.ShowWindow(new MyWindow());
        }
    }
}