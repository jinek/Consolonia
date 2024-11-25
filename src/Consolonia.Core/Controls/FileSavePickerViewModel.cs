using System;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePickerViewModel : PickerViewModelBase<FilePickerSaveOptions>
    {
        [ObservableProperty] private string _savePath=string.Empty;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedFile))]
        private IStorageItem _selectedItem;

        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            this.PropertyChanged += FileSavePickerViewModel_PropertyChanged; 
        }

        private void FileSavePickerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFile))
            {
                SavePath = SelectedFile.Path.LocalPath;
            }
        }

        public IStorageFile SelectedFile => SelectedItem as IStorageFile;

        protected override bool FilterItem(IStorageItem item)
        {
            return true;
        }
    }
}