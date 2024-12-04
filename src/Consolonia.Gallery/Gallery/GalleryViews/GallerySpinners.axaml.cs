using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GallerySpinners: UserControl
    {
        public GallerySpinners()
        {
            InitializeComponent();
            this.DataContext = new GallerySpinnersViewModel();
        }

        private void Button_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (GallerySpinnersViewModel)this.DataContext;
            model.IsBusy = !model.IsBusy;
        }
    }

    public partial class GallerySpinnersViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isBusy;
    }
}