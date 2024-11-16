using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnOpen(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
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
                    Patterns= new List<string> { "*.csproj" }
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
    }

    public ProjectViewModel Model => (ProjectViewModel)DataContext!;

    private void OnExit(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}

