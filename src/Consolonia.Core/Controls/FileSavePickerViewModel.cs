using System;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePickerViewModel : PickerViewModelBase<FilePickerSaveOptions>
    {
        [ObservableProperty] private string _savePath;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedFile))]
        private IStorageItem _selectedItem;

        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));
            this.PropertyChanged += FileSavePickerViewModel_PropertyChanged; ;    
        }

        private void FileSavePickerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedFile))
            {
                SavePath = SelectedFile?.Path.LocalPath;
            }
        }

        public IStorageFile SelectedFile => SelectedItem as IStorageFile;

        protected override bool FilterItem(IStorageItem item)
        {
            //if (item is IStorageFolder) return true;
            //if (item is IStorageFile file)
            //{
            //    // ReSharper disable ConstantConditionalAccessQualifier
            //    if (SelectedFileType == null)
            //        return true;

            //    foreach (string pattern in SelectedFileType.Patterns)
            //        if (file.Path.LocalPath.EndsWith(pattern.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
            //            return true;
            //    return false;
            //}

            return true;
        }
    }
}