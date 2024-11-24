using System;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePickerViewModel : PickerViewModelBase<FilePickerSaveOptions>
    {
        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {

        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedFile))]
        private IStorageItem _selectedItem;

        public IStorageFile SelectedFile => SelectedItem as IStorageFile;

        [ObservableProperty]
        private FilePickerFileType _selectedFileType;

        protected override bool FilterItem(IStorageItem item)
        {
            if (SelectedFileType != null)
            {
                if (item is IStorageFolder folder)
                {
                    return true;
                }
                if (item is IStorageFile file)
                {
                    foreach (var pattern in SelectedFileType.Patterns)
                    {
                        if (file.Path.LocalPath.EndsWith(pattern.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    return false;
                }
            }
            return true;
        }
    }
}