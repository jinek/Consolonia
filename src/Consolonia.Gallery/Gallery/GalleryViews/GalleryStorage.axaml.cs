using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Joins;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Infrastructure;
using Microsoft.VisualBasic.FileIO;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryStorage : UserControl
    {
        public GalleryStorage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.DataContext = new GalleryStorageViewModel();
        }

        private async void OnOpenFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IClassicDesktopStyleApplicationLifetime lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                var storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                    {
                        Title = "Open File",
                        SuggestedStartLocation = new SystemStorageFolder(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))),
                        FileTypeFilter = new List<FilePickerFileType>()
                        {
                             new FilePickerFileType("All files")
                             {
                                 Patterns = new List<string>() { "*" }
                             },
                             new FilePickerFileType("Text")
                             {
                                 Patterns = new List<string>() { "*.txt" }
                             },
                             new FilePickerFileType("Comma Delimited Files")
                             {
                                 Patterns = new List<string>() { "*.csv" }
                             },
                             new FilePickerFileType("PDF")
                             {
                                 Patterns = new List<string>() { "*.pdf" }
                             }
                        },
                    });

                    var model = (GalleryStorageViewModel)this.DataContext;
                    model.Files = files;
                }
            }
        }

        private async void OnOpenFolder(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IClassicDesktopStyleApplicationLifetime lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                var storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
                    {
                        Title = "Select a folder",
                        AllowMultiple = false
                    });
                    var model = (GalleryStorageViewModel)this.DataContext;
                    model.Folders = folders;
                }
            }
        }
        private async void OnSaveFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            IClassicDesktopStyleApplicationLifetime lifetime = App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
            {
                var storageProvider = lifetime.MainWindow.StorageProvider;
                if (storageProvider.CanOpen)
                {
                    var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
                    {
                        Title = "Save File",
                        SuggestedStartLocation = new SystemStorageFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
                        DefaultExtension = "txt",
                        SuggestedFileName = "NewFile.txt",
                        FileTypeChoices = new List<FilePickerFileType>()
                        {
                             new FilePickerFileType("Text")
                             {
                                 Patterns = new List<string>() { "*.txt" }
                             },
                             new FilePickerFileType("Comma delimited values")
                             {
                                 Patterns = new List<string>() { "*.csv" }
                             },
                        },
                    });

                    var model = (GalleryStorageViewModel)this.DataContext;
                    model.Files = new List<IStorageFile>() { file };
                }
            }
        }
    }

    public partial class GalleryStorageViewModel : ObservableObject
    {
        [ObservableProperty]
        private IReadOnlyList<IStorageFile> _files;

        [ObservableProperty]
        private IReadOnlyList<IStorageFolder> _folders;
    }
}