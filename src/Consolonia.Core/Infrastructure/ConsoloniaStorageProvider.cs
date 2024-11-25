using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using System.IO;
using Consolonia.Core.Controls;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaStorageProvider : IStorageProvider
    {
        public bool CanOpen => true;

        public bool CanSave => true;

        public bool CanPickFolder => true;


        public Task<IStorageBookmarkFile> OpenFileBookmarkAsync(string bookmark)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<IStorageFile>> OpenFilePickerAsync(FilePickerOpenOptions options)
        {
            var mainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime)?.MainWindow;
            ArgumentNullException.ThrowIfNull(mainWindow);

            var picker = new FileOpenPicker(options);
            var results = await picker.ShowDialogAsync<IReadOnlyList<IStorageFile>>(mainWindow);
            return results;
        }

        public Task<IStorageBookmarkFolder> OpenFolderBookmarkAsync(string bookmark)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<IStorageFolder>> OpenFolderPickerAsync(FolderPickerOpenOptions options)
        {
            var mainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime)?.MainWindow;
            ArgumentNullException.ThrowIfNull(mainWindow);

            var picker = new FolderPicker(options);
            var results = await picker.ShowDialogAsync<IReadOnlyList<IStorageFolder>>(mainWindow);
            return results;
        }

        public async Task<IStorageFile> SaveFilePickerAsync(FilePickerSaveOptions options)
        {
            var mainWindow = ((IClassicDesktopStyleApplicationLifetime)Application.Current?.ApplicationLifetime)?.MainWindow;
            ArgumentNullException.ThrowIfNull(mainWindow);

            var picker = new FileSavePicker(options);
            var results = await picker.ShowDialogAsync<IStorageFile>(mainWindow);
            return results;
        }

        public Task<IStorageFile> TryGetFileFromPathAsync(Uri filePath)
        {
            if (File.Exists(filePath.LocalPath))
                return Task.FromResult<IStorageFile>(new SystemStorageFile(filePath));
            return Task.FromResult<IStorageFile>(null);
        }

        public Task<IStorageFolder> TryGetFolderFromPathAsync(Uri folderPath)
        {
            if (Directory.Exists(folderPath.LocalPath))
                return Task.FromResult<IStorageFolder>(new SystemStorageFolder(folderPath));
            return Task.FromResult<IStorageFolder>(null);
        }

        public Task<IStorageFolder> TryGetWellKnownFolderAsync(WellKnownFolder wellKnownFolder)
        {
            var dir = wellKnownFolder switch
            {
                WellKnownFolder.Desktop => Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                WellKnownFolder.Documents => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                WellKnownFolder.Downloads => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                WellKnownFolder.Pictures => Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                WellKnownFolder.Music => Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                WellKnownFolder.Videos => Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                _ => null
            };
            if (String.IsNullOrEmpty(dir))
                return Task.FromResult<IStorageFolder>(null);
            return Task.FromResult<IStorageFolder>(new SystemStorageFolder(dir));
        }
    }
}
