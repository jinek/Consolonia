using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Infrastructure
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public class StorageFolder : IStorageFolder
    {
        private DirectoryInfo _directoryInfo;

        public StorageFolder(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public string Name => _directoryInfo.Name;

        public Uri Path => new Uri($"file://{_directoryInfo.FullName}");

        public bool CanBookmark => throw new NotImplementedException();

        public Task<IStorageFile> CreateFileAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IStorageFolder> CreateFolderAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<StorageItemProperties> GetBasicPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<IStorageItem> GetItemsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IStorageFolder> GetParentAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            throw new NotImplementedException();
        }

        public Task<string> SaveBookmarkAsync()
        {
            throw new NotImplementedException();
        }

        private string GetDebuggerDisplay()
        {
            return Path.LocalPath;
        }
    }
}
