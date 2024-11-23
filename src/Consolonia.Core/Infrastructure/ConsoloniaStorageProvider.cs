using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Consolonia.Core.Controls.Views;
using Consolonia.Core.Controls.ViewModels;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System.Linq;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaStorageProvider : IStorageProvider
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
            var mainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;

            var picker = new FileOpenPicker(options);
            var results = await picker.ShowDialogAsync<IStorageFile[]>(mainWindow).ConfigureAwait(false);
            return results;
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
