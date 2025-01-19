using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.PreviewHost.ViewModels
{
    public partial class XamlFileViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty] private Assembly _assembly;

        [ObservableProperty] private Control? _content;

        private bool _disposedValue;
        private FileSystemWatcher? _fileWatcher;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(Name))]
        private string? _fullName;

        [ObservableProperty] private string? _name;

        public XamlFileViewModel(string xamlPath, Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);
            ArgumentNullException.ThrowIfNull(xamlPath);

            FullName = xamlPath;
            Name = Path.GetFileName(xamlPath);
            Assembly = assembly;

            WatchFileChanges();
        }
        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~XamlFileViewModel()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Load()
        {
            Content = LoadXaml();
        }

        private Control LoadXaml()
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                string xaml = null!;
                int nTries = 0;
                while (xaml == null)
                    try
                    {
                        xaml = File.ReadAllText(FullName!);
                    }
                    catch (IOException)
                    {
                        if (nTries++ < 3)
                            Thread.Sleep(100);
                        else
                            throw;
                    }


                object content = AvaloniaRuntimeXamlLoader.Load(xaml, Assembly, designMode: true);
                if (content is TopLevel top)
                    // If the root element is a TopLevel, we can't use it as the content of the panel
                    if (top.Content != null)
                    {
                        // So we'll use the content of the TopLevel as the content of the panel
                        content = top.Content;
                        top.Content = null;
                    }

                if (content is Control control)
                {
                    Application.Current!.TryGetResource("ThemeBorderBrush", null, out object? borderBrush);
                    var panel = new Border
                    {
                        BorderThickness = new Thickness(0, 0, 0, 0),
                        BorderBrush = (IBrush)borderBrush!
                    };

                    panel.Child = control;
                    Design.ApplyDesignModeProperties(panel, control);

                    if (control.IsSet(Design.WidthProperty))
                    {
                        panel.Width = control.GetValue(Design.WidthProperty);
                        if (panel.Width > 150)
                            panel.Width /= 10;
                    }

                    if (control.IsSet(Design.HeightProperty))
                    {
                        panel.Height = control.GetValue(Design.HeightProperty);
                        if (panel.Height > 150)
                            panel.Height /= 10;
                    }

                    return panel;
                }

                return new TextBlock { Text = "Root element is not a control" };
            }
            catch (Exception e)
            {
                return new TextBlock { Text = e.Message };
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void WatchFileChanges()
        {
            if (_fileWatcher == null)
            {
                ArgumentNullException.ThrowIfNull(FullName);

                _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(FullName!)!, Path.GetFileName(FullName));

                _fileWatcher.Changed += (_, _) => Dispatcher.UIThread.Invoke(() => Content = LoadXaml());
                _fileWatcher.Renamed += (_, _) => Dispatcher.UIThread.Invoke(() => Content = LoadXaml());
                _fileWatcher.EnableRaisingEvents = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    if (_fileWatcher != null)
                    {
                        _fileWatcher.EnableRaisingEvents = false;
                        _fileWatcher.Dispose();
                        _fileWatcher = null;
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    _disposedValue = true;
                }
        }

        //private void WatchAssemblyChanges()
        //{
        //    _assemblyWatcher = new FileSystemWatcher(Path.GetDirectoryName(_assemblyPath)!, Path.GetFileName(_assemblyPath));
        //    _assemblyWatcher.Changed += (sender, e) =>
        //    {
        //        Dispatcher.UIThread.Invoke(() =>
        //        {
        //            var applicationLifetime = (ISingleViewApplicationLifetime)ApplicationLifetime!;
        //            _loadContext.Unload();
        //            _loadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(_assemblyPath)));

        //            applicationLifetime.MainWindow!.Content = LoadXaml();
        //        });
        //    };
        //    _assemblyWatcher.EnableRaisingEvents = true;
        //}
    }
}