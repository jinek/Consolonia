using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public AppViewModel Model => (AppViewModel)DataContext!;

        private async void OnOpen(object? sender, RoutedEventArgs e)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            ArgumentNullException.ThrowIfNull(Model.Project);
            ArgumentNullException.ThrowIfNull(Model.Project.ProjectPath);

            // Start async operation to open the dialog
            var uri = new Uri(Path.GetDirectoryName(Model.Project.ProjectPath)!);
            IStorageFolder? startLocation = await StorageProvider.TryGetFolderFromPathAsync(uri);

            IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open csproj",
                AllowMultiple = false,
                SuggestedStartLocation = startLocation,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new("C# Project")
                    {
                        Patterns = new List<string> { "*.csproj" }
                    }
                }
            });

            if (!files.Any()) return;
            string projectFile = files[0].Path.AbsolutePath;
            Model.Project = new ProjectViewModel(projectFile);
        }

        private void OnExit(object? sender, RoutedEventArgs e)
        {
            Application.Current!.ApplicationLifetime!.Shutdown();
        }
    }
}