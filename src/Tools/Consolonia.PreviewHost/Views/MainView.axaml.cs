using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public AppViewModel Model => (AppViewModel)DataContext!;

#if FILE_OPEN
        private async void OnOpen(object? sender, RoutedEventArgs e)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            var topLevel = TopLevel.GetTopLevel(this);

            // Start async operation to open the dialog.
            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open csproj",
                AllowMultiple = false,
                SuggestedStartLocation = new SystemStorageFolder(Path.GetDirectoryName(Model.Project!.ProjectPath)),
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
            var projectFile = files[0].Path.AbsolutePath; 
            Dispatcher.UIThread.Invoke(() =>
            {
                Model.Project = new ProjectViewModel(projectFile);
            });
        }
#endif
        private void OnExit(object? sender, RoutedEventArgs e)
        {
            Application.Current!.ApplicationLifetime!.Shutdown();
        }
    }
}