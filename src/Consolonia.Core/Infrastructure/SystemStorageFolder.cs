using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Infrastructure
{
    [DebuggerDisplay("Folder: {Name}")]
    public sealed class SystemStorageFolder : IStorageFolder
    {
        private readonly DirectoryInfo _directoryInfo;
        private readonly bool _isParent;

        public SystemStorageFolder(string path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        public SystemStorageFolder(Uri uri)
        {
            _directoryInfo = new DirectoryInfo(uri.LocalPath);
        }

        public SystemStorageFolder(DirectoryInfo directoryInfo, bool isParent = false)
        {
            _isParent = isParent;
            _directoryInfo = directoryInfo;
        }

        public string Name => _isParent ? ".." : _directoryInfo.Name;

        public Uri Path => new($"file://{_directoryInfo.FullName}");

        public bool CanBookmark => false;

        public async Task<IStorageFile> CreateFileAsync(string name)
        {
            await Task.CompletedTask;

            string path = System.IO.Path.Combine(_directoryInfo.FullName, name);
            using (FileStream stream = File.Create(path))
            {
                await stream.WriteAsync(Array.Empty<byte>().AsMemory(0, 0));
            }

            return new SystemStorageFile(path);
        }

        public Task<IStorageFolder> CreateFolderAsync(string name)
        {
            DirectoryInfo dirInfo = Directory.CreateDirectory(System.IO.Path.Combine(_directoryInfo.FullName, name));
            return Task.FromResult((IStorageFolder)new SystemStorageFolder(dirInfo));
        }

        public Task DeleteAsync()
        {
            return Task.Run(() => _directoryInfo.Delete());
        }

        public void Dispose()
        {
        }

        public Task<StorageItemProperties> GetBasicPropertiesAsync()
        {
            var properties = new StorageItemProperties(dateCreated: _directoryInfo.CreationTime,
                dateModified: _directoryInfo.LastAccessTime);
            return Task.FromResult(properties);
        }

        public async IAsyncEnumerable<IStorageItem> GetItemsAsync()
        {
            await Task.CompletedTask;

            if (_directoryInfo.Exists)
            {
                foreach (DirectoryInfo folder in _directoryInfo.GetDirectories())
                    yield return new SystemStorageFolder(folder);

                foreach (FileInfo file in _directoryInfo.GetFiles()) yield return new SystemStorageFile(file);
            }
        }

        public Task<IStorageFolder> GetParentAsync()
        {
            if (_directoryInfo.Parent == null)
                return Task.FromResult((IStorageFolder)null);

            return Task.FromResult((IStorageFolder)new SystemStorageFolder(_directoryInfo.Parent));
        }

        public Task<IStorageItem> MoveAsync(IStorageFolder destination)
        {
            string targetPath = System.IO.Path.Combine(destination.Path.LocalPath, _directoryInfo.Name);
            _directoryInfo.MoveTo(targetPath);
            return Task.FromResult((IStorageItem)new SystemStorageFolder(targetPath));
        }

        public Task<string> SaveBookmarkAsync()
        {
            throw new NotImplementedException();
        }
    }
}