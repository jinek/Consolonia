using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure
{
    public sealed class AsyncDataTransfer : IAsyncDataTransfer
    {
        private List<IAsyncDataTransferItem> _items;
        private bool _disposedValue;

        public AsyncDataTransfer()
        {
            _items = new List<IAsyncDataTransferItem>();
        }

        public AsyncDataTransfer(IAsyncDataTransferItem item)
        {
            _items = [item];
        }

        public AsyncDataTransfer(IEnumerable<IAsyncDataTransferItem> items)
        {
            _items = items.ToList();
        }

        public IReadOnlyList<DataFormat> Formats => _items[0].Formats;

        public IReadOnlyList<IAsyncDataTransferItem> Items => _items;

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in Items)
                    {
                        if (item is IDisposable disposable)
                            disposable.Dispose();
                    }
                }

                _disposedValue = true;
            }
        }

        ~AsyncDataTransfer()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public class AsyncDataTransferItem : IAsyncDataTransferItem,
        IDataObject
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private readonly List<DataFormat> _formats;
        private readonly object _item;

        public AsyncDataTransferItem(object item, params DataFormat[] formats)
        {
            _formats = formats.ToList();
            _item = item;
        }

        #region IAsyncDataTransferItem Members
        public IReadOnlyList<DataFormat> Formats => _formats;

        public Task<object> TryGetRawAsync(DataFormat format)
        {
            if (_formats.Contains(format))
                return Task.FromResult(_item);
            return Task.FromResult<object>(null);
        }
        #endregion

        #region IDataObject
        public bool Contains(string dataFormat)
        {
            return _formats.Any(f => f.Identifier == dataFormat);
        }

        public object Get(string dataFormat)
        {
            if (Contains(dataFormat))
                return _item;
            return null;
        }

        public IEnumerable<string> GetDataFormats()
        {
            return _formats.Select(f => f.Identifier);
        }
        #endregion
    }
}
