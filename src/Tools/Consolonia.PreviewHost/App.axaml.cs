using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Consolonia.Core.Infrastructure;
using Consolonia.PreviewHost.Views;
using Consolonia.PreviewHost.ViewModels;
using Consolonia.Themes.TurboVision.Templates;

namespace Consolonia.PreviewHost
{
    public partial class App : ConsoloniaApplication
    {
        static App()
        {
            // we want tests and UI to be executed with same culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public App()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            IClassicDesktopStyleApplicationLifetime applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            if (applicationLifetime != null)
            {
                var appViewModel = new AppViewModel();
                
                var path = applicationLifetime.Args!.FirstOrDefault();
                if (!String.IsNullOrEmpty(path))
                {
                    path = Path.GetFullPath(path);
                    var projectFile = FindProjectFileFromPath(path);

                    appViewModel.Project = new ProjectViewModel(projectFile);
                    if (path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                    {
                        applicationLifetime!.MainWindow = new HeadlessWindow()
                        {
                            DataContext = appViewModel.Project.Files.Single(f => f.FullName!.Equals(path, StringComparison.OrdinalIgnoreCase))
                        };
                    }
                }
                else
                {
                    var projectFile = FindProjectFileFromPath(Environment.CurrentDirectory);
                    if (projectFile != null)
                    {
                        appViewModel.Project = new ProjectViewModel(projectFile);
                    }
                }

                if (applicationLifetime.MainWindow == null)
                {
                    applicationLifetime!.MainWindow = new MainWindow()
                    {
                        DataContext = appViewModel
                    };
                }

                base.OnFrameworkInitializationCompleted();
            }

        }

        public static string FindProjectFileFromPath(string path)
        {
            ArgumentNullException.ThrowIfNull(path);
            string? projectFile = null;
            string projectFolder = Directory.Exists(path) ? path : Path.GetDirectoryName(path)!;
            while (projectFolder != null)
            {
                projectFile = Directory.GetFiles(projectFolder, "*.csproj").FirstOrDefault();
                if (projectFile != null)
                {
                    break;
                }
                projectFolder = Path.GetDirectoryName(projectFolder)!;
            }
            ArgumentNullException.ThrowIfNull(projectFile);
            return projectFile;
        }



        //        window.KeyDown += (sender, e) =>
        //            {
        //                if (e.Key == Key.Escape)
        //                {
        //                    if (_fileWatcher != null)
        //                    {
        //                        _fileWatcher.EnableRaisingEvents = false;
        //                        _fileWatcher.Dispose();
        //                    }

        //                    if (_assemblyWatcher != null)
        //                    {
        //                        _assemblyWatcher.EnableRaisingEvents = false;
        //                        _assemblyWatcher.Dispose();
        //                    }

        //                    ((Window)sender!).Close();
        //                }
        //                else if (e.Key == Key.Space)
        //{
        //    Dispatcher.UIThread.Invoke(() =>
        //    {
        //        var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
        //        applicationLifetime.MainWindow!.Content = LoadXaml();
        //    });
        //}
        //            };

    }
}
