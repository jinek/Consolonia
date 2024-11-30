using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.PreviewHost.ViewModels;
using Avalonia.Threading;

namespace Avalonia.PreviewHost.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainViewModel Model => (MainViewModel)DataContext!;

        private async void OnOpen(object? sender, RoutedEventArgs e)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            TopLevel? topLevel = GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open csproj",
                AllowMultiple = false,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("C# Project")
                    {
                        Patterns = new List<string> { "*.csproj" }
                    }
                }
            }).ConfigureAwait(false);

            if (!files.Any()) return;

            Dispatcher.UIThread.Invoke(() =>
            {
                Model.Files.Clear();
                string projectFile = files[0].Path.AbsolutePath.Replace('/', '\\');
                Title = projectFile;

                string folderRoot = Path.GetDirectoryName(projectFile)!;
                foreach (string file in Directory.EnumerateFiles(folderRoot, "*.axaml", SearchOption.AllDirectories))
                    Model.Files.Add(new FileViewModel
                    {
                        Name = Path.GetFileName(file),
                        FullName = file
                    });
            });
        }

        private void OnExit(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}