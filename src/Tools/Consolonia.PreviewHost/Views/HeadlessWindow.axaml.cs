using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core.Infrastructure;
using Consolonia.PreviewHost.ViewModels;
using Newtonsoft.Json;

namespace Consolonia.PreviewHost.Views;

public partial class HeadlessWindow : Window
{
    public HeadlessWindow()
    {
        InitializeComponent();
    }

    public AppViewModel Model => (AppViewModel)DataContext!;

    private void OnKeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Escape)
            this.Close();
        else if (e.Key == Avalonia.Input.Key.Left)
        {
            var i = Model.Project.Files.IndexOf(Model.Project.Current);
            if (i > 0)
                Model.Project.Current = Model.Project.Files[i - 1];
        }
        else if (e.Key == Avalonia.Input.Key.Right)
        {
            var i = Model.Project.Files.IndexOf(Model.Project.Current);
            if (i < Model.Project.Files.Count - 1)
                Model.Project.Current = Model.Project.Files[i + 1];
        }
    }


    private void ContentControl_DataContextChanged(object? sender, System.EventArgs e)
    {
        base.OnDataContextEndUpdate();

        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        if (lifetime.Args!.Contains("--buffer"))
        {
            Dispatcher.UIThread.Post(() =>
            {
                var consoleWindow = this.PlatformImpl as ConsoleWindow;
                ArgumentNullException.ThrowIfNull(consoleWindow);
                var json = JsonConvert.SerializeObject(consoleWindow.PixelBuffer);
                Console.WriteLine(json);
            });
        }
    }
}

