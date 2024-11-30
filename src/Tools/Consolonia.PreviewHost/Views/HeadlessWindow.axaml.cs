using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using Consolonia.Core.Infrastructure;
using Consolonia.PreviewHost.ViewModels;
using Newtonsoft.Json;

namespace Consolonia.PreviewHost.Views
{
    public partial class HeadlessWindow : Window
    {
        public HeadlessWindow()
        {
            InitializeComponent();
        }

        public AppViewModel Model => (AppViewModel)DataContext!;

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
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
            base.OnDataContextEndUpdate();

            var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            if (lifetime.Args!.Contains("--buffer"))
                Dispatcher.UIThread.Post(() =>
                {
                    var consoleWindow = PlatformImpl as ConsoleWindow;
                    ArgumentNullException.ThrowIfNull(consoleWindow);
                    string json = JsonConvert.SerializeObject(consoleWindow.PixelBuffer);
                    Console.WriteLine(json);
                });
        }
    }
}