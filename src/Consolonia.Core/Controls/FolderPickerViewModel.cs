using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FolderPickerViewModel : PickerViewModelBase<FolderPickerOpenOptions>
    {
        public FolderPickerViewModel(FolderPickerOpenOptions options)
            : base(options)
        {
            this.SelectionMode = options.AllowMultiple ? SelectionMode.Multiple : SelectionMode.Single;
        }

        [ObservableProperty]
        private ObservableCollection<IStorageFolder> _selectedFolders = new ObservableCollection<IStorageFolder>();

        [ObservableProperty]
        private SelectionMode _selectionMode;

        [ObservableProperty]
        private bool _hasSelection;

        protected override bool FilterItem(IStorageItem item)
        {
            return item is IStorageFolder;
        }
    }
}