using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls
{

    public abstract partial class PickerViewModelBase<TPickerOptions> : ObservableObject
        where TPickerOptions : PickerOptions
    {
        protected PickerViewModelBase(TPickerOptions options)
        {
            Options = options;
            CurrentFolderPath = options.SuggestedStartLocation?.Path.LocalPath ?? Environment.CurrentDirectory;
            CurrentFolder = options.SuggestedStartLocation;
            _ = LoadCurrentFolder();
            PropertyChanged += PickerViewModel_PropertyChanged;
        }


        [ObservableProperty]
        private TPickerOptions _options;

        [ObservableProperty]
        private string _currentFolderPath;

        [ObservableProperty]
        private IStorageFolder _currentFolder;

        [ObservableProperty]
        private ObservableCollection<IStorageItem> _items = new ObservableCollection<IStorageItem>();


        private async void PickerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentFolderPath):
                    try
                    {
                        CurrentFolder = new SystemStorageFolder(new DirectoryInfo(this.CurrentFolderPath));
                    }
                    catch (DirectoryNotFoundException)
                    {
                        // ignore
                    }
                    break;

                case nameof(CurrentFolder):
                    await LoadCurrentFolder();
                    break;
            }
        }

        private async Task LoadCurrentFolder()
        {
            Items.Clear();

            this.Items.Add(new SystemStorageFolder(new DirectoryInfo(Path.Combine(CurrentFolder.Path.LocalPath, "..")), true));

            await foreach (var item in CurrentFolder.GetItemsAsync())
            {
                if (this.FilterItem(item))
                    this.Items.Add(item);
            }
        }

        protected abstract bool FilterItem(IStorageItem item);
    }
}
