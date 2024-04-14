using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryDialog : UserControl
    {
        public GalleryDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // ReSharper disable UnusedParameter.Local
        private async void Button_OnClick(object _, RoutedEventArgs e)
            // ReSharper restore UnusedParameter.Local
        {
            await new SomeDialogWindow(50, 15).ShowDialogAsync(this);
        }
    }
}