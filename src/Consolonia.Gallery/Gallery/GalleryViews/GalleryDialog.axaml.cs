using Avalonia;
using Avalonia.Controls;
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
            var lifetime = (ConsoloniaLifetime)Application.Current.ApplicationLifetime;

            var dialog = new SomeDialogWindow(50, 15);
            await dialog.ShowDialog();
        }
    }
}