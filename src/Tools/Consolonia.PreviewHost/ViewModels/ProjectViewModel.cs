using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Loader;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.PreviewHost.ViewModels;

public partial class ProjectViewModel : ObservableObject
{
    private static readonly AssemblyLoadContext _loadContext = new CustomAssemblyLoadContext();

    public ProjectViewModel(string projectFile)
    {
        _project = projectFile;

        var projectFolder = Path.GetDirectoryName(_project)!;
        ArgumentNullException.ThrowIfNull(_project);
        var projectName = Path.GetFileNameWithoutExtension(_project);
        var assemblyName = Path.GetFileNameWithoutExtension(_project) + ".dll";
        var buildDirectory = Path.Combine(projectFolder!, "bin", "Debug");
        var assemblyPath = Directory.EnumerateFiles(buildDirectory, assemblyName, SearchOption.AllDirectories).First();


        ArgumentNullException.ThrowIfNull(assemblyPath);
        // load assembly
        Assembly = _loadContext!.LoadFromStream(new MemoryStream(File.ReadAllBytes(assemblyPath)));
        ArgumentNullException.ThrowIfNull(Assembly);

        foreach (var xamlFile in Directory.GetFiles(projectFolder, "*.axaml", SearchOption.AllDirectories))
        {
            _files.Add(new XamlFileViewModel(xamlFile, Assembly));
        }
    }

    [ObservableProperty]
    private string? _project;

    [ObservableProperty]
    private ObservableCollection<XamlFileViewModel> _files = new ObservableCollection<XamlFileViewModel>();

    public Assembly Assembly { get; set; }

}


