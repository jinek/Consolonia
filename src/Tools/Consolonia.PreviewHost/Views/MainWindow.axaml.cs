using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Consolonia.Core.Controls;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        public AppViewModel Model => (AppViewModel)DataContext!;

        private async void OnOpen(object? sender, RoutedEventArgs e)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);

            ArgumentNullException.ThrowIfNull(Model.Project);
            ArgumentNullException.ThrowIfNull(Model.Project.ProjectPath);
            
            // Start async operation to open the dialog
            var uri = new Uri(Path.GetDirectoryName(Model.Project.ProjectPath)!);
            var startLocation = await topLevel!.StorageProvider.TryGetFolderFromPathAsync(uri);

            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open csproj",
                AllowMultiple = false,
                SuggestedStartLocation = startLocation,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    new FilePickerFileType("C# Project")
                    {
                        Patterns = new List<string> { "*.csproj" }
                    },
                },
            });

            if (files == null || !files.Any())
            {
                return;
            }
            var projectFile = files[0].Path.AbsolutePath;
            Model.Project = new ProjectViewModel(projectFile);
        }

        private void OnExit(object? sender, RoutedEventArgs e)
        {
            Application.Current!.ApplicationLifetime!.Shutdown();
        }
    }
}