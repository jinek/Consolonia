using System;
using System.ComponentModel;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    internal partial class FileSavePickerViewModel : PickerViewModelBase<FilePickerSaveOptions>
    {
        [ObservableProperty] 
        private string _savePath = string.Empty;

        public bool HasSelection => SelectedItem != null;

        [ObservableProperty] 
        [NotifyPropertyChangedFor(nameof(SelectedFile))]
        [NotifyPropertyChangedFor(nameof(HasSelection))]
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        private IStorageItem? _selectedItem;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

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