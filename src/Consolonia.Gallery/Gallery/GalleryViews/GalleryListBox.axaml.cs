using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    [GalleryOrder(70)]
    public class GalleryListBox : UserControl
    {
        public GalleryListBox()
        {
            InitializeComponent();
            DataContext = new ListBoxPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}