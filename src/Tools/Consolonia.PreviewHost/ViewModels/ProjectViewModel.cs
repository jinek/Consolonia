using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Loader;
using Avalonia;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.PreviewHost.ViewModels
{
    public partial class ProjectViewModel : ObservableObject
    {
        private static readonly AssemblyLoadContext LoadContext = new CustomAssemblyLoadContext();

        [ObservableProperty] private XamlFileViewModel? _current;

        [ObservableProperty] private ObservableCollection<XamlFileViewModel> _files = new();

        [ObservableProperty] private string? _projectPath;

        public ProjectViewModel(string projectFile)
        {
            _projectPath = projectFile;

            string projectFolder = Path.GetDirectoryName(_projectPath)!;
            ArgumentNullException.ThrowIfNull(_projectPath);
            string assemblyName = Path.GetFileNameWithoutExtension(_projectPath) + ".dll";
            string buildDirectory = Path.Combine(projectFolder, "bin", "Debug");
            string assemblyPath = Directory.EnumerateFiles(buildDirectory, assemblyName, SearchOption.AllDirectories)
                .First();

            ArgumentNullException.ThrowIfNull(assemblyPath);
            // load assembly
            Assembly = LoadContext.LoadFromStream(new MemoryStream(File.ReadAllBytes(assemblyPath)));
            ArgumentNullException.ThrowIfNull(Assembly);

            foreach (string xamlFile in Directory.GetFiles(projectFolder, "*.axaml", SearchOption.AllDirectories))
                _files.Add(new XamlFileViewModel(xamlFile, Assembly));

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Current))
                    if (Current != null)
                        Current.Load();
            };

            var lifetime = (ConsoloniaLifetime)Application.Current!.ApplicationLifetime!;
            if (lifetime.Args!.Contains("--buffer"))
                _ = Task.Run(() =>
                {
                    string? xamlFile = Console.ReadLine();
                    while (!string.IsNullOrEmpty(xamlFile))
                    {
                        if (!string.IsNullOrEmpty(xamlFile) &&
                            xamlFile.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase))
                        {
                            XamlFileViewModel file =
                                _files.SingleOrDefault(f =>
                                    f.FullName!.Equals(xamlFile, StringComparison.OrdinalIgnoreCase))
                                ?? _files.SingleOrDefault(f =>
                                    f.Name!.Equals(Path.GetFileName(xamlFile), StringComparison.OrdinalIgnoreCase))
                                ?? throw new ArgumentException($"{xamlFile} not found in project");

                            Dispatcher.UIThread.Invoke(() => Current = file);
                        }

                        xamlFile = Console.ReadLine();
                    }
                });
        }

        public Assembly Assembly { get; set; }
    }
}