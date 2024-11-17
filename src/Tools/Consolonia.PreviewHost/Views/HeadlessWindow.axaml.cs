using Avalonia;
using System.Linq;
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

    public ProjectViewModel Model => (ProjectViewModel)DataContext!;

    private void OnKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        this.Close();
    }

    protected override void OnDataContextEndUpdate()
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

