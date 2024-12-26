using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryMessageBoxViewModel : ObservableObject
    {
        [ObservableProperty] private string _result;
    }

    public partial class GalleryMessageBox : UserControl
    {
        public GalleryMessageBox()
        {
            InitializeComponent();
            DataContext = new GalleryMessageBoxViewModel();
        }

        private GalleryMessageBoxViewModel ViewModel => (GalleryMessageBoxViewModel)DataContext;

        private async void OnOk(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                Mode = Mode.Ok,
                Title = "OK Message box"
            };
            MessageBoxResult result = await mb.ShowDialogAsync(this, "This is a message");
            ViewModel.Result = result.ToString();
        }

        private async void OnOkCancel(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                Mode = Mode.OkCancel,
                Title = "Ok/Cancel Message box"
            };
            MessageBoxResult result = await mb.ShowDialogAsync(this, "Do you want to OK or cancel?");
            ViewModel.Result = result.ToString();
        }

        private async void OnYesNo(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                Mode = Mode.YesNo,
                Title = "Yes/No Message box"
            };
            MessageBoxResult result = await mb.ShowDialogAsync(this, "Do you want to?");
            ViewModel.Result = result.ToString();
        }

        private async void OnYesNoCancel(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                Mode = Mode.YesNoCancel,
                Title = "Yes/No/Cancel Message box"
            };
            MessageBoxResult result = await mb.ShowDialogAsync(this, "Do you want to, or cancel?");
            ViewModel.Result = result.ToString();
        }

        private async void OnCustom(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                Mode = Mode.YesNoCancel,
                Title = "Custom OK content Message box",
                Yes = new TextBlock
                {
                    Text = "😁 Yes",
                    Foreground = Brushes.Lime
                },
                No = new TextBlock
                {
                    Text = "😒 No",
                    Foreground = Brushes.Red
                }
            };
            MessageBoxResult result = await mb.ShowDialogAsync(this, "Custom OK button");
            ViewModel.Result = result.ToString();
        }
    }
}