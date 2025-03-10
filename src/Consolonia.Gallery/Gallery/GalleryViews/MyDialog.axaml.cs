using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Avalonia.Interactivity;

namespace Consolonia.Gallery.Gallery.GalleryViews;


public partial class MyDialog : Consolonia.Controls.Window
{
    private static int _dialogCount = 0;
    private static IImmutableSolidColorBrush[] _brushes =
         [
             Brushes.LightBlue,
            Brushes.LightGreen,
            Brushes.LightCyan,
            Brushes.LightSalmon,
            Brushes.LightSeaGreen,
            Brushes.LightSlateGray,
            Brushes.LightCoral,
            Brushes.LightGoldenrodYellow,
            Brushes.LightPink
         ];

    public MyDialog()
    {
        InitializeComponent();
        // this.Background = brushes[Random.Shared.Next(0, brushes.Length)];
        AnimateWindow = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("CONSOLONIA_TEST"));

        this.DataContext = new MyDialogViewModel()
        {
            Title = $"New Dialog {++_dialogCount}"
        };
    }

    public MyDialogViewModel ViewModel => (MyDialogViewModel)DataContext;

    private void OnOK(object sender, RoutedEventArgs args)
    {
        this.Close(ViewModel.Text);
    }

    private void OnCancel(object sender, RoutedEventArgs args)
    {
        this.Close(null);
    }

    private void OnColor(object sender, RoutedEventArgs args)
    {
        this.Background = _brushes[Random.Shared.Next(0, _brushes.Length)];
    }
}

public partial class MyDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    private string _title = "New Dialog";
}
