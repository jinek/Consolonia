using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryDialog : UserControl
    {
        public GalleryDialog()
        {
            InitializeComponent();
        }

        // ReSharper disable UnusedParameter.Local
        private async void Button_OnClick(object _, RoutedEventArgs e)
            // ReSharper restore UnusedParameter.Local
        {
            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime;
            await new SomeDialogWindow(50, 15).ShowDialogAsync(lifetime.MainWindow);
        }
    }
}