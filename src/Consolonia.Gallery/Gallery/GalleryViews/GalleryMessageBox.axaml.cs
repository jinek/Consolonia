using System;
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
            MessageBoxResult result = await MessageBox.ShowDialog(this, "OK Message box", "Do you want to OK?");
            ViewModel.Result = result.ToString();
        }

        private async void OnOkCancel(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                MessageBoxStyle = MessageBoxStyle.OkCancel,
                Title = "OK/Cancel Message box",
                Message = "Do you want to OK or cancel?",
                AnimateWindow = !Extensions.IsUnitTestConsole()
            };
            MessageBoxResult result = await mb.ShowDialog(this);
            ViewModel.Result = result.ToString();
        }

        private async void OnYesNo(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                MessageBoxStyle = MessageBoxStyle.YesNo,
                Title = "Yes/No Message box",
                Message = "Do you want to Yes or No?",
                AnimateWindow = !Extensions.IsUnitTestConsole()
            };
            MessageBoxResult result = await mb.ShowDialog(this);
            ViewModel.Result = result.ToString();
        }

        private async void OnYesNoCancel(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                MessageBoxStyle = MessageBoxStyle.YesNoCancel,
                Title = "Yes/No/Cancel Message box",
                Message = "Do you want to Yes, No or Cancel?",
                AnimateWindow = !Extensions.IsUnitTestConsole()
            };
            MessageBoxResult result = await mb.ShowDialog(this);
            ViewModel.Result = result.ToString();
        }

        private async void OnCustom(object sender, RoutedEventArgs e)
        {
            var mb = new MessageBox
            {
                MessageBoxStyle = MessageBoxStyle.YesNoCancel,
                Title = "Custom OK content Message box",
                Message = new TextBlock
                {
                    Text = " Message",
                    Foreground = Brushes.Purple
                },
                Yes = new TextBlock
                {
                    Text = "😁 Yes",
                    Foreground = Brushes.Lime
                },
                No = new TextBlock
                {
                    Text = "😒 No",
                    Foreground = Brushes.Red
                },
                AnimateWindow = !Extensions.IsUnitTestConsole()
            };
            MessageBoxResult result = await mb.ShowDialog(this);
            ViewModel.Result = result.ToString();
        }
    }
}