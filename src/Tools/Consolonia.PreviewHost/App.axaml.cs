using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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
            IClassicDesktopStyleApplicationLifetime applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
            if (applicationLifetime != null)
            {
                var path = applicationLifetime.Args!.FirstOrDefault();
                Window window;
                var projectFile = FindProjectFileFromPath(path);
                var projectViewModel = new ProjectViewModel(projectFile);

                if (path.EndsWith(".axaml"))
                {
                    applicationLifetime!.MainWindow = new HeadlessWindow()
                    {
                        Foreground = Brushes.White,
                        Background = Brushes.Black,
                        DataContext = projectViewModel.Files.Single(f => f.FullName.Equals(path, StringComparison.OrdinalIgnoreCase) )
                    };
                }
                else
                {
                    applicationLifetime!.MainWindow = new ProjectWindow()
                    {
                        Foreground = Brushes.White,
                        Background = Brushes.Black,
                        DataContext = projectViewModel
                    };
                }

                base.OnFrameworkInitializationCompleted();
            }

        }

        public static string FindProjectFileFromPath(string path)
        {
            string? projectFile = null;
            string projectFolder = Path.GetDirectoryName(path!);
            while (projectFolder != null)
            {
                projectFile = Directory.GetFiles(projectFolder, "*.csproj").FirstOrDefault();
                if (projectFile != null)
                {
                    break;
                }
                projectFolder = Path.GetDirectoryName(projectFolder)!;
            }
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

        //private void WatchAssemblyChanges()
        //        {
        //            _assemblyWatcher = new FileSystemWatcher(Path.GetDirectoryName(_assemblyPath)!, Path.GetFileName(_assemblyPath));
        //            _assemblyWatcher.Changed += (sender, e) =>
        //            {
        //                Dispatcher.UIThread.Invoke(() =>
        //                {
        //                    var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
        //                    _loadContext.Unload();
        //                    _loadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(_assemblyPath)));

        //                    applicationLifetime.MainWindow!.Content = LoadXaml();
        //                });
        //            };
        //            _assemblyWatcher.EnableRaisingEvents = true;
        //        }

        //        private void WatchFileChanges()
        //        {
        //            if (_fileWatcher == null)
        //            {
        //                ArgumentNullException.ThrowIfNull(_xamlPath);

        //                _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_xamlPath!)!, Path.GetFileName(_xamlPath));

        //                _fileWatcher.Changed += (e, s) => RefreshPreview();
        //                _fileWatcher.Renamed += (e, s) => RefreshPreview();
        //                _fileWatcher.EnableRaisingEvents = true;
        //            }
        //        }

        //        private void RefreshPreview()
        //        {
        //            Dispatcher.UIThread.Invoke(() =>
        //            {
        //                var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
        //                applicationLifetime.MainWindow!.Content = LoadXaml();
        //            });
        //        }
    }
}
