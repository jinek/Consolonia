using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Infrastructure
{
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public sealed class SystemStorageFolder : IStorageFolder
    {
        private DirectoryInfo _directoryInfo;
        private bool _isParent;

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

        public string Name => (_isParent) ? ".." : _directoryInfo.Name;

        public Uri Path => new Uri($"file://{_directoryInfo.FullName}");

        public bool CanBookmark => false;

        public async Task<IStorageFile> CreateFileAsync(string name)
        {
            await Task.CompletedTask;

            var path = System.IO.Path.Combine(_directoryInfo.FullName, name);
            using (var stream = File.Create(path))
            {
            }
            return new SystemStorageFile(path);
        }

        public Task<IStorageFolder> CreateFolderAsync(string name)
        {
            var dirInfo = Directory.CreateDirectory(System.IO.Path.Combine(_directoryInfo.FullName, name));
            return Task.FromResult((IStorageFolder)new SystemStorageFolder(dirInfo));
        }

        public Task DeleteAsync()
        {
            _directoryInfo.Delete();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }

        public Task<StorageItemProperties> GetBasicPropertiesAsync()
        {
            var properties = new StorageItemProperties(dateCreated: _directoryInfo.CreationTime, dateModified: _directoryInfo.LastAccessTime);
            return Task.FromResult(properties);
        }

        public async IAsyncEnumerable<IStorageItem> GetItemsAsync()
        {
            await Task.CompletedTask;

            if (_directoryInfo.Exists)
            {
                yield return new SystemStorageFolder(_directoryInfo.Parent, isParent: true);

                foreach (var folder in _directoryInfo.GetDirectories())
                {
                    yield return new SystemStorageFolder(folder);
                }

                foreach (var file in _directoryInfo.GetFiles())
                {
                    yield return new SystemStorageFile(file);
                }
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
            _directoryInfo.MoveTo(destination.Path.LocalPath);
            return Task.FromResult((IStorageItem)new SystemStorageFolder(new DirectoryInfo(destination.Path.LocalPath)));
        }

        public Task<string> SaveBookmarkAsync()
        {
            throw new NotImplementedException();
        }

        private string GetDebuggerDisplay()
        {
            return _directoryInfo.FullName;
        }
    }
}
