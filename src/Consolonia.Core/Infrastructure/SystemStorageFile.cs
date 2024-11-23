using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Infrastructure
{
    public class SystemStorageFile : IStorageFile
    {
        private FileInfo _fileInfo;

        public SystemStorageFile(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public string Name => _fileInfo.Name;

        public Uri Path => new Uri($"file://{_fileInfo.FullName}");

        public bool CanBookmark => false;

        public Task DeleteAsync()
        {
            _fileInfo.Delete();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public Task<StorageItemProperties> GetBasicPropertiesAsync()
        {
            var result = new StorageItemProperties((ulong)_fileInfo.Length, _fileInfo.CreationTime, _fileInfo.LastWriteTime);
            return Task.FromResult(result);
        }

        public Task<IStorageFolder> GetParentAsync()
        {
            IStorageFolder result = new SystemStorageFolder(_fileInfo.Directory!);
            return Task.FromResult(result);
        }

        public Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            var path = destination.Path.LocalPath;
            _fileInfo.MoveTo(path);
            return Task.FromResult((IStorageItem)this);
        }

        public async Task<Stream> OpenReadAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return _fileInfo.OpenRead();
        }

        public async Task<Stream> OpenWriteAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
            return _fileInfo.OpenWrite();
        }

        public Task<string> SaveBookmarkAsync()
        {
            throw new NotImplementedException();
        }
    }
}
