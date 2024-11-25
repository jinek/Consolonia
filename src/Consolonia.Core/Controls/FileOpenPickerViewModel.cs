using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FileOpenPickerViewModel : PickerViewModelBase<FilePickerOpenOptions>
    {
        public FileOpenPickerViewModel(FilePickerOpenOptions options)
            : base(options)
        {
            this.SelectionMode = options.AllowMultiple ? SelectionMode.Multiple : SelectionMode.Single;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentFolderPath))]
        private int _selectedFilterIndex;

        [ObservableProperty]
        private SelectionMode _selectionMode;

        [ObservableProperty]
        private ObservableCollection<IStorageFile> _selectedFiles = new ObservableCollection<IStorageFile>();

        [ObservableProperty]
        private bool _hasSelection;

        protected override bool FilterItem(IStorageItem item)
        {
            if (Options.FileTypeFilter == null || Options.FileTypeFilter.Count == 0)
                return true;

            if (item is IStorageFile file)
            {
                var selectedFileType = Options.FileTypeFilter[SelectedFilterIndex]!;
                if (selectedFileType.Patterns == null)
                    return true;

                foreach (var pattern in selectedFileType.Patterns)
                {
                    if (file.Path.LocalPath.EndsWith(pattern.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            // show folders
            return true;
        }

    }
}