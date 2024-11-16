using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Consolonia.Core.Designer;
using PreviewHost.ViewModels;

namespace PreviewHost.Views;

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
        });

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
            Model.Files.Add(new FileViewModel()
            {
                Name = Path.GetFileName(file), 
                FullName = file
            });
        }
    }

    public MainViewModel Model => (MainViewModel)DataContext!;

    private void OnExit(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}

