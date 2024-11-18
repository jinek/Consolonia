using System.Globalization;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Consolonia.Core.Infrastructure;
using Consolonia.PreviewHost.Views;
using Consolonia.PreviewHost.ViewModels;

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
            IClassicDesktopStyleApplicationLifetime? applicationLifetime = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (applicationLifetime != null)
            {
                var appViewModel = new AppViewModel();

                var path = applicationLifetime.Args!.FirstOrDefault();
                if (!String.IsNullOrEmpty(path))
                {
                    string folder;
                    if (Path.IsPathFullyQualified(path))
                    {
                        folder = Path.GetDirectoryName(path)!;
                    }
                    else
                        folder = Environment.CurrentDirectory;
                    ArgumentNullException.ThrowIfNull(folder);
                    var projectFile = FindProjectFileFromPath(folder);

                    appViewModel.Project = new ProjectViewModel(projectFile);
                    if (path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                    {
                        applicationLifetime!.MainWindow = new HeadlessWindow()
                        {
                            DataContext = appViewModel.Project.Files.SingleOrDefault(f => f.FullName!.Equals(path, StringComparison.OrdinalIgnoreCase))
                                ?? appViewModel.Project.Files.SingleOrDefault(f => f.Name!.Equals(Path.GetFileName(path), StringComparison.OrdinalIgnoreCase))
                                ?? throw new ArgumentException($"{path} not found in project", nameof(path))
                        };
                    }
                }
                else
                {
                    var projectFile = FindProjectFileFromPath(Environment.CurrentDirectory);
                    appViewModel.Project = new ProjectViewModel(projectFile);
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
            return projectFile!;
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