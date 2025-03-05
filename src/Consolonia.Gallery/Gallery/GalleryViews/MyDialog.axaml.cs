using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Iciclecreek.Avalonia.WindowManager;
using System;
using Avalonia.Interactivity;

namespace Consolonia.Gallery.Gallery.GalleryViews;

public partial class MyDialog : ManagedWindow
{
    private static int _dialogCount = 0;
    private static IImmutableSolidColorBrush[] brushes =
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
        this.Background = brushes[Random.Shared.Next(0, brushes.Length)];

        this.DataContext = new MyDialogViewModel()
        {
            Title = $"New Dialog {++_dialogCount}"
        };
    }

    public MyDialogViewModel ViewModel => (MyDialogViewModel)DataContext;

    private void OnOK(object? sender, RoutedEventArgs args)
    {
        this.Close(ViewModel.Text);
    }

    private void OnCancel(object? sender, RoutedEventArgs args)
    {
        this.Close(null);
    }

    private void OnColor(object? sender, RoutedEventArgs args)
    {
        this.Background = brushes[Random.Shared.Next(0, brushes.Length)];
    }
}

public partial class MyDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    private string _title = "New Dialog";
}
