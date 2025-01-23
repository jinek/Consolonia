using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public ProjectViewModel Model => (ProjectViewModel)DataContext!;

        private async void OnOpen(object? sender, RoutedEventArgs e)
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
                    Patterns = new List<string> { "*.csproj" }
                },
            },
            }).ConfigureAwait(false);

            if (files == null || !files.Any())
            {
                return;
            }
            Model.Files.Clear();
            var projectFile = files[0].Path.AbsolutePath.Replace('/', '\\');
            //this.Title = projectFile;

            var folderRoot = Path.GetDirectoryName(projectFile)!;
            foreach (var file in Directory.EnumerateFiles(folderRoot, "*.axaml", SearchOption.AllDirectories))
            {
                Model.Files.Add(new XamlFileViewModel(file, null));
            }
        }

        private void OnExit(object? sender, RoutedEventArgs e)
        {
            Application.Current!.ApplicationLifetime!.Shutdown();
        }
    }
}