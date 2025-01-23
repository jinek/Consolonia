using System.Globalization;
using Avalonia;
using Consolonia.PreviewHost.ViewModels;
using Consolonia.PreviewHost.Views;
using Consolonia.Themes;

namespace Consolonia.PreviewHost
{
    public class App : Application
    {
        static App()
        {
            // we want tests and UI to be executed with same culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public App()
        {
            Styles.Add(new MaterialTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var applicationLifetime = ApplicationLifetime as ConsoloniaLifetime;
            if (applicationLifetime?.Args != null)
            {
                var appViewModel = new AppViewModel();

                string? path = applicationLifetime.Args.FirstOrDefault();
                if (!string.IsNullOrEmpty(path))
                {
                    string folder;
                    if (Path.IsPathFullyQualified(path))
                    {
                        if (Directory.Exists(path))
                            folder = path;
                        else
                            folder = Path.GetDirectoryName(path)!;
                    }
                    else
                    {
                        folder = Environment.CurrentDirectory;
                    }

                    ArgumentNullException.ThrowIfNull(folder);
                    string projectFile = FindProjectFileFromPath(folder);

                    appViewModel.Project = new ProjectViewModel(projectFile);
                    if (path.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                    {
                        appViewModel.Project.Current =
                            appViewModel.Project.Files.SingleOrDefault(f =>
                                f.FullName!.Equals(path, StringComparison.OrdinalIgnoreCase))
                            ?? appViewModel.Project.Files.SingleOrDefault(f =>
                                f.Name!.Equals(Path.GetFileName(path), StringComparison.OrdinalIgnoreCase))
                            ?? throw new ArgumentException($"{path} not found in project", nameof(path));
                    }
                    applicationLifetime.MainView = new MainView()
                    {
                        DataContext = appViewModel
                    };
                }
                else
                {
                    string projectFile = FindProjectFileFromPath(Environment.CurrentDirectory);
                    appViewModel.Project = new ProjectViewModel(projectFile);
                    applicationLifetime.MainView = new MainView()
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
            string? projectFolder = Directory.Exists(path) ? path : Path.GetDirectoryName(path);
            while (projectFolder != null)
            {
                projectFile = Directory.GetFiles(projectFolder, "*.csproj").FirstOrDefault();
                if (projectFile != null) break;
                projectFolder = Path.GetDirectoryName(projectFolder);
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