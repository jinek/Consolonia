using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Iciclecreek.Avalonia.WindowManager;
using System;
using Avalonia.Interactivity;
using Avalonia;

namespace Consolonia.Gallery.Gallery.GalleryViews;

public partial class MyWindow : Consolonia.Controls.Window
{
    private static int _windowCount = 0;
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

    public MyWindow()
    {
        InitializeComponent();

        //  this.Background = brushes[Random.Shared.Next(0, brushes.Length)];
        AnimateWindow = String.IsNullOrEmpty(Environment.GetEnvironmentVariable("CONSOLONIA_TEST"));

        this.DataContext = new MyWindowViewModel()
        {
            Counter = 0,
            Title = $"New Window {++_windowCount}"
        };
    }

    public MyWindowViewModel ViewModel => (MyWindowViewModel)DataContext;

    private void OnIncrement(object sender, RoutedEventArgs args)
    {
        var vm = (MyWindowViewModel)this.DataContext;
        if (vm != null)
        {
            vm.Counter++;
        }
    }

    private void OnColor(object sender, RoutedEventArgs args)
    {
        this.Background = _brushes[Random.Shared.Next(0, _brushes.Length)];
    }

    private void OnClose(object sender, RoutedEventArgs args)
    {
        this.Close();
    }


    private async void OnShowDialog(object sender, RoutedEventArgs args)
    {
        var dialog = new MyDialog()
        {
            WindowStartupLocation = Enum.Parse<WindowStartupLocation>((StartupLocationCombo.SelectedItem as ComboBoxItem).Tag.ToString()),
            SizeToContent = Enum.Parse<SizeToContent>(((ComboBoxItem)SizeToContentCombo.SelectedItem).Tag.ToString()),
        };
        dialog.ViewModel.Text = ViewModel.Text;

        dialog.SizeToBounds(this.Bounds);

        var result = await dialog.ShowDialog<string?>(this);
        if (result != null)
            ViewModel.Text = result;
    }
}

public partial class MyWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private int _counter = 0;

    [ObservableProperty]
    private string _text = string.Empty;

    [ObservableProperty]
    private string _title = "New Window";
}

internal static class Utils
{
    public static void SizeToBounds(this ManagedWindow window, Rect rect)
    {
        var minWidth = (int)rect.Width / 4;
        var minHeight = (int)rect.Height / 4;
        var maxWidth = (int)rect.Width / 2;
        var maxHeight = (int)rect.Height / 2;

        if (window.WindowStartupLocation == WindowStartupLocation.Manual)
        {
            window.Position = new PixelPoint(Random.Shared.Next(0, (int)rect.Width - maxWidth),
                                          Random.Shared.Next(0, (int)rect.Height - maxHeight));
        }

        switch (window.SizeToContent)
        {
            case Avalonia.Controls.SizeToContent.Manual:
                // we are providing both width and height
                window.Width = Random.Shared.Next(minWidth, maxWidth);
                window.Height = Random.Shared.Next(minHeight, maxHeight);
                break;
            case Avalonia.Controls.SizeToContent.Width:
                // we are expecting window to figure out width
                window.Height = Random.Shared.Next(minHeight, maxHeight);
                break;
            case Avalonia.Controls.SizeToContent.Height:
                // we are expecting window to figure out height
                window.Width = Random.Shared.Next(minWidth, maxWidth);
                break;
            case Avalonia.Controls.SizeToContent.WidthAndHeight:
                // we are expecting window to figure out both width and height
                break;
        }
    }
}