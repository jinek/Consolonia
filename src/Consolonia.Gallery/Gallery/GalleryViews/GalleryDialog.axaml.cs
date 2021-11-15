using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
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

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            new SomeDialogWindow().ShowPrivate(this);
        }
    }
}