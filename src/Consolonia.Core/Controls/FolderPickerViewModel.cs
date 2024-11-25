using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FolderPickerViewModel : PickerViewModelBase<FolderPickerOpenOptions>
    {
        [ObservableProperty] private bool _hasSelection;

        [ObservableProperty] private ObservableCollection<IStorageFolder> _selectedFolders = new();

        [ObservableProperty] private SelectionMode _selectionMode;

        public FolderPickerViewModel(FolderPickerOpenOptions options)
            : base(options)
        {
            SelectionMode = options.AllowMultiple ? SelectionMode.Multiple : SelectionMode.Single;
        }

        protected override bool FilterItem(IStorageItem item)
        {
            return item is IStorageFolder;
        }
    }
}