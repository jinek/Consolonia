using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls.ViewModels
{

    public partial class PickerViewModel<TPickerOptions> : ObservableObject
        where TPickerOptions : PickerOptions
    {
        public PickerViewModel(TPickerOptions options)
        {
            Options = options;
            PropertyChanged += PickerViewModel_PropertyChanged;
            CurrentFolder = options.SuggestedStartLocation;
        }


        [ObservableProperty]
        private TPickerOptions _options;

        [ObservableProperty]
        private IStorageFolder _currentFolder;

        [ObservableProperty]
        private ObservableCollection<IStorageItem> _items = new ObservableCollection<IStorageItem>();

        [ObservableProperty]
        private IStorageItem _selectedItem;

        private async void PickerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CurrentFolder):
                    Items.Clear();

                    await foreach (var item in CurrentFolder.GetItemsAsync())
                    {
                        if (item is IStorageFolder)
                            this.Items.Add(item);
                    }

                    await foreach (var item in CurrentFolder.GetItemsAsync())
                    {
                        if (item is IStorageItem)
                            this.Items.Add(item);
                    }
                    break;
            }
        }
    }
}
