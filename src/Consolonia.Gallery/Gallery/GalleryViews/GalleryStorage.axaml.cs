using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryStorageViewModel : ObservableObject
    {
        [ObservableProperty]
        private IReadOnlyList<IStorageFile> _files;

        [ObservableProperty]
        private IReadOnlyList<IStorageFolder> _folders;
    }

    public partial class GalleryStorage : UserControl
    {
        public GalleryStorage()
        {
            InitializeComponent();
        }

        private GalleryStorageViewModel ViewModel => (GalleryStorageViewModel)DataContext;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.DataContext = new GalleryStorageViewModel();
        }

        private async void OnOpenFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await OpenFiles("Open file", allowMultiple: false);
        }
        private async void OnOpenMultipleFiles(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await OpenFiles("Open files", allowMultiple: true);
        }

        private async Task OpenFiles(string title, bool allowMultiple)
        {
            IClassicDesktopStyleApplicationLifetime lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                var storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                    {
                        Title = title,
                        AllowMultiple = allowMultiple,
                        SuggestedStartLocation = new SystemStorageFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))),
                        FileTypeFilter = new List<FilePickerFileType>()
                        {
                             new FilePickerFileType("All files") { Patterns = new[] { "*" } },
                             new FilePickerFileType("Text") { Patterns = new[] { "*.txt" } },
                             new FilePickerFileType("Comma Delimited Files") { Patterns = new[] { "*.csv" } },
                             new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } }
                        },
                    });

                    ViewModel.Files = files;
                }
            }
        }

        private async void OnOpenFolder(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await OpenFolders(title: "Select a folder", allowMultiple: false);
        }

        private async void OnOpenMultipleFolders(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await OpenFolders(title: "Select folder(s)", allowMultiple: true);
        }

        private async Task OpenFolders(string title, bool allowMultiple)
        {
            IClassicDesktopStyleApplicationLifetime lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                var storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                    {
                        Title = title,
                        SuggestedStartLocation = new SystemStorageFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))),
                        AllowMultiple = allowMultiple
                    });
                    ViewModel.Folders = folders;
                }
            }
        }

        private async void OnSaveFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IClassicDesktopStyleApplicationLifetime lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                var storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanSave)
                {
                    var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                    {
                        Title = "Save File",
                        SuggestedStartLocation = new SystemStorageFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                        DefaultExtension = "txt",
                        SuggestedFileName = "NewFile.txt",
                        FileTypeChoices = new List<FilePickerFileType>()
                        {
                             new FilePickerFileType("Text") { Patterns = new[] { "*.txt" } },
                             new FilePickerFileType("Comma Delimited Files") { Patterns = new[] { "*.csv" } },
                             new FilePickerFileType("PDF") { Patterns = new[] { "*.pdf" } }
                        },
                    });

                    ViewModel.Files = new List<IStorageFile>() { file };
                }
            }
        }
    }

}