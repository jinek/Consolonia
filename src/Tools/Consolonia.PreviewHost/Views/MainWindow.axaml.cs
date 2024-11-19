using Avalonia.Controls;
using Avalonia.Interactivity;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public ProjectViewModel Model => (ProjectViewModel)DataContext!;

        private void OnOpen(object? sender, RoutedEventArgs e)
        {
#if IStorageProvider
        // Get top level from the current control. Alternatively, you can use Window reference instead.
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open csproj",
            AllowMultiple = false,
            FileTypeFilter = new List<FilePickerFileType>
            {
                new FilePickerFileType("C# Project")
                {
                    Patterns = new List<string> { "*.csproj" }
                },
            },
        }).ConfigureAwait(false);

        if (files == null || !files.Any())
        {
            return;
        }
        Model.Files.Clear();
        var projectFile = files[0].Path.AbsolutePath.Replace('/','\\');
        this.Title = projectFile;
        
        var folderRoot = Path.GetDirectoryName(projectFile)!;
        foreach (var file in Directory.EnumerateFiles(folderRoot, "*.axaml", SearchOption.AllDirectories))
        {
            Model.Files.Add(new XamlFileViewModel(file, null));
        }
#endif
        }

        private void OnExit(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}