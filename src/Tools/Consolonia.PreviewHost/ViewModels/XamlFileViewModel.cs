using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;
using System.Xml;

namespace Consolonia.PreviewHost.ViewModels;

public partial class XamlFileViewModel : ObservableObject, IDisposable
{
    private bool _loaded;
    private FileSystemWatcher? _fileWatcher;
    private bool _disposedValue;

    public XamlFileViewModel(string xamlPath, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(xamlPath);

        FullName = xamlPath;
        Name = Path.GetFileName(xamlPath);
        Assembly = assembly;

        WatchFileChanges();
    }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Name))]
    private string? _fullName;

    [ObservableProperty]
    private Assembly _assembly;

    private Control? _content;
    public Control? Content
    {
        get
        {
            if (!_loaded)
            {
                _content = LoadXAML();
                _loaded = true;
            }
            return _content;
        }
        set => SetProperty(ref _content, value);
    }

    public Control LoadXAML()
    {
        try
        {
            string xaml = null!;
            int nTries = 0;
            while (xaml == null)
            {
                try
                {
                    xaml = File.ReadAllText(FullName!);
                }
                catch (IOException)
                {
                    if (nTries++ < 3)
                    {

                        Thread.Sleep(100);
                        continue;
                    }
                    else
                        throw;
                }
            }


            var content = AvaloniaRuntimeXamlLoader.Load(xaml, Assembly, designMode: true);
            if (content is Control control)
            {
                var stackPanel = new StackPanel();
                stackPanel.HorizontalAlignment = HorizontalAlignment.Left;
                stackPanel.VerticalAlignment = VerticalAlignment.Top;
                stackPanel.Children.Add(control);
                Design.ApplyDesignModeProperties(stackPanel, control);

                return stackPanel;
            }
            else
            {
                return new TextBlock() { Text = "Root element is not a control" };
            }
        }
        catch (XmlException e)
        {
            return new TextBlock() { Text = e.Message };
        }
    }

    private void WatchFileChanges()
    {
        if (_fileWatcher == null)
        {
            ArgumentNullException.ThrowIfNull(FullName);

            _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(FullName!)!, Path.GetFileName(FullName));

            _fileWatcher.Changed += (e, s) => Dispatcher.UIThread.Invoke(() => Content = LoadXAML());
            _fileWatcher.Renamed += (e, s) => Dispatcher.UIThread.Invoke(() => Content = LoadXAML());
            _fileWatcher.EnableRaisingEvents = true;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
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
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    //private void WatchAssemblyChanges()
    //{
    //    _assemblyWatcher = new FileSystemWatcher(Path.GetDirectoryName(_assemblyPath)!, Path.GetFileName(_assemblyPath));
    //    _assemblyWatcher.Changed += (sender, e) =>
    //    {
    //        Dispatcher.UIThread.Invoke(() =>
    //        {
    //            var applicationLifetime = (IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!;
    //            _loadContext.Unload();
    //            _loadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(_assemblyPath)));

    //            applicationLifetime.MainWindow!.Content = LoadXaml();
    //        });
    //    };
    //    _assemblyWatcher.EnableRaisingEvents = true;
    //}

}

