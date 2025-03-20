using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryStorageViewModel : ObservableObject
    {
        [ObservableProperty] private IReadOnlyList<IStorageFile> _files;

        [ObservableProperty] private IReadOnlyList<IStorageFolder> _folders;
    }

    public partial class GalleryStorage : UserControl
    {
        public GalleryStorage()
        {
            InitializeComponent();
            DataContext = new GalleryStorageViewModel();
        }

        private GalleryStorageViewModel ViewModel => (GalleryStorageViewModel)DataContext;

        private async void OnOpenFile(object sender, RoutedEventArgs e)
        {
            await OpenFiles("Open file", false);
        }

        private async void OnOpenMultipleFiles(object sender, RoutedEventArgs e)
        {
            await OpenFiles("Open files", true);
        }

        private async Task OpenFiles(string title, bool allowMultiple)
        {
            IStorageProvider storageProvider = TopLevel.GetTopLevel(this).StorageProvider;
            if (storageProvider.CanOpen)
            {
                IStorageFolder startLocation =
                    await storageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory);
                var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = title,
                    AllowMultiple = allowMultiple,
                    SuggestedStartLocation = startLocation,
                    FileTypeFilter = new List<FilePickerFileType>
                    {
                        new("* files") { Patterns = ["*"] },
                        new("*.* files") { Patterns = ["*.*"] },
                        new("Text") { Patterns = ["*.txt"] },
                        new("Comma Delimited Files") { Patterns = ["*.csv"] },
                        new("PDF") { Patterns = ["*.pdf"] }
                    }
                });

                ViewModel.Files = files;
            }
        }

        private async void OnOpenFolder(object sender, RoutedEventArgs e)
        {
            await OpenFolders("Select a folder", false);
        }

        private async void OnOpenMultipleFolders(object sender, RoutedEventArgs e)
        {
            await OpenFolders("Select folder(s)", true);
        }

        private async Task OpenFolders(string title, bool allowMultiple)
        {
            if (Application.Current.ApplicationLifetime is ConsoloniaLifetime lifetime)
            {
                IStorageProvider storageProvider = lifetime.TopLevel.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    IStorageFolder startLocation =
                        await storageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory);

                    var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = title,
                        SuggestedStartLocation = startLocation,
                        AllowMultiple = allowMultiple
                    });
                    ViewModel.Folders = folders;
                }
            }
        }

        private async void OnSaveFile(object sender, RoutedEventArgs e)
        {
            IStorageProvider storageProvider = TopLevel.GetTopLevel(this).StorageProvider;
            if (storageProvider != null && storageProvider.CanSave)
            {
                IStorageFolder startLocation =
                    await storageProvider.TryGetFolderFromPathAsync(Environment.CurrentDirectory);

                IStorageFile file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save File",
                    SuggestedStartLocation = startLocation,
                    DefaultExtension = "txt",
                    SuggestedFileName = "NewFile.txt",
                    FileTypeChoices = new List<FilePickerFileType>
                    {
                        new("Text") { Patterns = ["*.txt"] },
                        new("Comma Delimited Files") { Patterns = ["*.csv"] },
                        new("PDF") { Patterns = ["*.pdf"] }
                    }
                });

                ViewModel.Files = new List<IStorageFile> { file };
            }
        }
    }
}