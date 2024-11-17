using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(10)]
    public partial class GalleryWelcome : UserControl
    {
        public GalleryWelcome()
        {
            this.DataContext = new TestModel();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnAdd(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (TestModel)this.DataContext;
            model.Glyphs.Insert(0, "X");
        }

        private void OnRemove(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (TestModel)this.DataContext;
            model.Glyphs.RemoveAt(0);
        }
    }

    public partial class TestModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> glyphs = new ObservableCollection<string>()
        {
            "中"
        };
    }
}