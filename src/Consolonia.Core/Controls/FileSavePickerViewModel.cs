using System;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePickerViewModel : PickerViewModelBase<FilePickerSaveOptions>
    {
        [ObservableProperty] private FilePickerFileType _selectedFileType;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedFile))]
        private IStorageItem _selectedItem;

        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
        }

        public IStorageFile SelectedFile => SelectedItem as IStorageFile;

        protected override bool FilterItem(IStorageItem item)
        {
            if (item is IStorageFolder) return true;
            if (item is IStorageFile file)
            {
                if (SelectedFileType?.Patterns == null || SelectedFileType.Patterns.Count == 0)
                    return true;

                foreach (string pattern in SelectedFileType.Patterns)
                    if (file.Path.LocalPath.EndsWith(pattern.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                        return true;
                return false;
            }

            return true;
        }
    }
}