using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Previewer
{
    public partial class App : ConsoloniaApplication
    {
        private string _xamlPath;
        private string _assemblyPath;
        private Assembly _assembly;
        private FileSystemWatcher? _assemblyWatcher = null;
        private FileSystemWatcher? _fileWatcher = null;
        private readonly AssemblyLoadContext? _loadContext = new CustomAssemblyLoadContext();

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
            IClassicDesktopStyleApplicationLifetime applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime;
            if (applicationLifetime != null)
            {
                _xamlPath = applicationLifetime.Args.First();
                
                InitializePreview();

                var control = LoadXaml();

                Window window = CreatePreviewWindow(control);

                applicationLifetime!.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private Window CreatePreviewWindow(Control content)
        {
            var window = new Window()
            {
                Content = content,
                Foreground = Brushes.White,
                Background = Brushes.Black,
            };

            window.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    if (_fileWatcher != null)
                    {
                        _fileWatcher.EnableRaisingEvents = false;
                        _fileWatcher.Dispose();
                    }

                    if (_assemblyWatcher != null)
                    {
                        _assemblyWatcher.EnableRaisingEvents = false;
                        _assemblyWatcher.Dispose();
                    }

                    ((Window)sender).Close();
                }
                else if (e.Key == Key.Space)
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
                        applicationLifetime.MainWindow!.Content = LoadXaml();
                    });
                }
            };
            return window;
        }

        private void InitializePreview()
        {
            string projectFile = null;
            string projectFolder = Path.GetDirectoryName(_xamlPath);
            while (projectFolder != null)
            {
                projectFile = Directory.GetFiles(projectFolder, "*.csproj").FirstOrDefault();
                if (projectFile != null)
                {
                    break;
                }
                projectFolder = Path.GetDirectoryName(projectFolder);
            }
            ArgumentNullException.ThrowIfNull(projectFile);
            var projectName = Path.GetFileNameWithoutExtension(projectFile);
            var assemblyName = Path.GetFileNameWithoutExtension(projectFile) + ".dll";
            var buildDirectory = Path.Combine(projectFolder, "bin", "Debug");
            _assemblyPath = Directory.EnumerateFiles(buildDirectory, assemblyName, SearchOption.AllDirectories).First();

            WatchAssemblyChanges();
            WatchFileChanges();
        }

        private Control LoadXaml()
        {
            string xaml = File.ReadAllText(_xamlPath);

            var control = (Control?)AvaloniaRuntimeXamlLoader.Load(xaml, _assembly, designMode: true);

            var stackPanel = new StackPanel();
            stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
            stackPanel.VerticalAlignment = VerticalAlignment.Top;
            stackPanel.Children.Add(control);
            Design.ApplyDesignModeProperties(stackPanel, control);

            return stackPanel;
        }

        private void WatchAssemblyChanges()
        {
            // load assembly
            _assembly = _loadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(_assemblyPath)));

            _assemblyWatcher = new FileSystemWatcher(Path.GetDirectoryName(_assemblyPath), Path.GetFileName(_assemblyPath));
            _assemblyWatcher.Changed += (sender, e) =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
                    _loadContext.Unload();
                    _loadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(_assemblyPath)));

                    applicationLifetime.MainWindow!.Content = LoadXaml();
                });
            };
            _assemblyWatcher.EnableRaisingEvents = true;
        }

        private void WatchFileChanges()
        {
            if (_fileWatcher == null)
            {
                _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_xamlPath), Path.GetFileName(_xamlPath));
                _fileWatcher.Changed += (sender, e) =>
                {
                    Dispatcher.UIThread.Invoke(() =>
                    {
                        var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
                        applicationLifetime.MainWindow!.Content = LoadXaml();
                    });
                };
                _fileWatcher.EnableRaisingEvents = true;
            }
        }

    }

    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {

        public CustomAssemblyLoadContext() : base(isCollectible: true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {

            return null; // Return null to use the default loading mechanism
        }
    }
    //// var subPath = Path.GetRelativePath(projectFolder, _xamlPath).Replace(Path.DirectorySeparatorChar, '.').Replace(".axaml", String.Empty);
    ////var resourceName = $"{projectName}.{subPath}";
    ////            var control = (Control?)Activator.CreateInstance(localAsm.ExportedTypes.Single(t => t.FullName == resourceName));
    //var subPath = Path.GetRelativePath(projectFolder, _xamlPath).Replace(Path.DirectorySeparatorChar, '/');
    //var resourceName = $"{projectName}/{subPath}";
    //var uri = new Uri($"avares://{resourceName}");

}
