using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

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
                    var results = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
                    {
                        Title = "Open File",
                        FileTypeFilter = new List<FilePickerFileType>()
                        {
                             new FilePickerFileType("Text") 
                             {
                                 Patterns = new List<string>() { "*.txt" } 
                             } 
                        },
                    });
                    if (results.Any())
                    {

                    }
                }
            }
        }

        private void OnOpenFolder(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        { }
        private void OnSaveFile(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
        }
    }

    public partial class GalleryStorageViewModel : ObservableObject
    {
        [ObservableProperty]
        private IStorageFile _file;

        [ObservableProperty]
        private IStorageFolder _folder;
    }
}