using System;
using System.ComponentModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePickerViewModel : PickerViewModelBase<FilePickerSaveOptions>
    {
        [ObservableProperty] private string _savePath = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedFile))]
        private IStorageItem _selectedItem;

        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            PropertyChanged += FileSavePickerViewModel_PropertyChanged;
        }

        public IStorageFile SelectedFile => SelectedItem as IStorageFile;

        private void FileSavePickerViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFile)) SavePath = SelectedFile?.Path.LocalPath ?? string.Empty;
        }

        protected override bool FilterItem(IStorageItem item)
        {
            return true;
        }
    }
}