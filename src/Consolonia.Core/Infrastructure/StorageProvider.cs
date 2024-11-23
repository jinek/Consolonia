using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Consolonia.Core.Controls.Views;
using Consolonia.Core.Controls.ViewModels;

namespace Consolonia.Core.Infrastructure
{
    public class StorageProvider : IStorageProvider
    {
        public bool CanOpen => true;

        public bool CanSave => true;

        public bool CanPickFolder => true;


        public async Task<IStorageBookmarkFile> OpenFileBookmarkAsync(string bookmark)
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return null;
        }

        public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            var model = new FileOpenPickerViewModel(options);
            var picker = new FileOpenPicker(50,50)
            {
                DataContext = model
            };
            await picker.ShowDialog(null);
            return new List<IStorageFile>() { (IStorageFile)model.SelectedItem };
        }

        public Task<IStorageBookmarkFolder> OpenFolderBookmarkAsync(string bookmark)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions options)
        {
            throw new NotImplementedException();
        }

        public Task<IStorageFile> TryGetFileFromPathAsync(Uri filePath)
        {
            throw new NotImplementedException();
        }

        public Task<IStorageFolder> TryGetFolderFromPathAsync(Uri folderPath)
        {
            throw new NotImplementedException();
        }

        public Task<IStorageFolder> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder)
        {
            throw new NotImplementedException();
        }
    }
}
