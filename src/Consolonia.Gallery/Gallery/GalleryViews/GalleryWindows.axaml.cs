using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;
using Iciclecreek.Avalonia.WindowManager;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryWindows : UserControl
    {
        public GalleryWindows()
        {
            InitializeComponent();
        }

        private void OnShowWindow(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var x = new ManagedWindow()
            {
                Title = "This is a window",
                Background = Brushes.AliceBlue,
                Content = new TextBlock()
                {
                    Text = "Hello, World!"
                },
                CanResize = true,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowState = WindowState.Normal,
                ShowActivated = true
            };
            var wm = TopLevel.GetTopLevel(this).FindDescendantOfType<WindowManagerPanel>();
            wm.ShowWindow(x);
        }
    }
}