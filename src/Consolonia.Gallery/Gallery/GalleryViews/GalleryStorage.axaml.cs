using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Infrastructure;

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
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                IStorageProvider storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                    {
                        Title = title,
                        AllowMultiple = allowMultiple,
                        SuggestedStartLocation = new SystemStorageFolder(
                            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))),
                        FileTypeFilter = new List<FilePickerFileType>
                        {
                            new("All files") { Patterns = ["*"] },
                            new("Text") { Patterns = ["*.txt"] },
                            new("Comma Delimited Files") { Patterns = ["*.csv"] },
                            new("PDF") { Patterns = ["*.pdf"] }
                        }
                    });

                    ViewModel.Files = files;
                }
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
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                IStorageProvider storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                    {
                        Title = title,
                        SuggestedStartLocation = new SystemStorageFolder(
                            new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))),
                        AllowMultiple = allowMultiple
                    });
                    ViewModel.Folders = folders;
                }
            }
        }

        private async void OnSaveFile(object sender, RoutedEventArgs e)
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                IStorageProvider storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanSave)
                {
                    IStorageFile file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        Title = "Save File",
                        SuggestedStartLocation =
                            new SystemStorageFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
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
}