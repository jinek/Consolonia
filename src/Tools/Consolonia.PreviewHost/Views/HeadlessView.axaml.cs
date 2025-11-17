using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Consolonia.Core.Infrastructure;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views
{
    public partial class HeadlessView : Window
    {
        public HeadlessView()
        {
            InitializeComponent();
        }

        public AppViewModel Model => (AppViewModel)DataContext!;

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Application.Current!.ApplicationLifetime!.Shutdown();
            }
            else if (e.Key == Key.Left)
            {
                if (Model.Project != null && Model.Project.Current != null)
                {
                    int i = Model.Project.Files.IndexOf(Model.Project.Current);
                    if (i >= 0)
                        Model.Project.Current = Model.Project.Files[i - 1];
                }
            }
            else if (e.Key == Key.Right)
            {
                if (Model.Project != null && Model.Project.Current != null)
                {
                    int i = Model.Project.Files.IndexOf(Model.Project.Current);
                    if (i >= 0 && i < Model.Project.Files.Count - 1)
                        Model.Project.Current = Model.Project.Files[i + 1];
                }
            }
        }


        private void ContentControl_DataContextChanged(object? sender, EventArgs e)
        {
            base.OnDataContextChanged(e);

            var lifetime = (ConsoloniaLifetime)Application.Current!.ApplicationLifetime!;
            if (lifetime.Args!.Contains("--buffer"))
                Dispatcher.UIThread.Post(() =>
                {
                    var consoleTopLevelImpl = lifetime.MainWindow?.PlatformImpl as ConsoleWindowImpl;
                    ArgumentNullException.ThrowIfNull(consoleTopLevelImpl);
                    string json = JsonSerializer.Serialize(consoleTopLevelImpl.PixelBuffer);
                    Console.WriteLine(json);
                });
        }
    }
}