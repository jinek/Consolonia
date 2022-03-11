using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    public class GalleryDialog : UserControl
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
            await new SomeDialogWindow().ShowDialogAsync(this);
        }
    }
}