using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    internal partial class FolderPickerViewModel : PickerViewModelBase<FolderPickerOpenOptions>
    {
        [NotifyPropertyChangedFor(nameof(HasSelection))] [ObservableProperty]
        private ObservableCollection<IStorageFile> _selectedFiles = new();

        [ObservableProperty] private ObservableCollection<IStorageFolder> _selectedFolders = new();

        [ObservableProperty] private SelectionMode _selectionMode;

        public FolderPickerViewModel(FolderPickerOpenOptions options)
            : base(options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            SelectionMode = options.AllowMultiple ? SelectionMode.Multiple : SelectionMode.Single;
            SelectedFiles.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasSelection));
        }

        public bool HasSelection => SelectedFolders.Any();

        protected override bool FilterItem(IStorageItem item)
        {
            return item is IStorageFolder;
        }
    }
}