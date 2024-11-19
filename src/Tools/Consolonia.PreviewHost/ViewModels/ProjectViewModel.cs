using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Loader;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.PreviewHost.ViewModels;

public partial class ProjectViewModel : ObservableObject
{
    private static readonly AssemblyLoadContext LoadContext = new CustomAssemblyLoadContext();

    public ProjectViewModel(string projectFile)
    {
        _project = projectFile;

        var projectFolder = Path.GetDirectoryName(_project)!;
        ArgumentNullException.ThrowIfNull(_project);
        var assemblyName = Path.GetFileNameWithoutExtension(_project) + ".dll";
        var buildDirectory = Path.Combine(projectFolder, "bin", "Debug");
        var assemblyPath = Directory.EnumerateFiles(buildDirectory, assemblyName, SearchOption.AllDirectories).First();

        ArgumentNullException.ThrowIfNull(assemblyPath);
        // load assembly
        Assembly = LoadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(assemblyPath)));
        ArgumentNullException.ThrowIfNull(Assembly);

        foreach (var xamlFile in Directory.GetFiles(projectFolder, "*.axaml", SearchOption.AllDirectories))
        {
            _files.Add(new XamlFileViewModel(xamlFile, Assembly));
        }

        this.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Current))
            {
                if (Current != null)
                {
                    Current.Load();
                }
            }
        };

        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        if (lifetime.Args!.Contains("--buffer"))
        {
            _ = Task.Run(() =>
            {
                string? xamlFile = Console.ReadLine();
                while (!String.IsNullOrEmpty(xamlFile))
                {
                    if (!String.IsNullOrEmpty(xamlFile) && xamlFile.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            this.Current = _files.SingleOrDefault(f => f.FullName!.Equals(xamlFile, StringComparison.OrdinalIgnoreCase))
                                ?? _files.SingleOrDefault(f => f.Name!.Equals(Path.GetFileName(xamlFile), StringComparison.OrdinalIgnoreCase))
                                ?? throw new ArgumentException($"{xamlFile} not found in project");
                        });
                    }

                    xamlFile = Console.ReadLine();
                }
            });
        }
    }

    [ObservableProperty]
    private string? _project;

    [ObservableProperty]
    private ObservableCollection<XamlFileViewModel> _files = new ObservableCollection<XamlFileViewModel>();

    [ObservableProperty]
    private XamlFileViewModel? _current;

    public Assembly Assembly { get; set; }

}


