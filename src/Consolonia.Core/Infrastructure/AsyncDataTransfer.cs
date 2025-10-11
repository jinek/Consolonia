using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure
{
    public sealed class AsyncDataTransfer : IAsyncDataTransfer
    {
        private readonly List<IAsyncDataTransferItem> _items;

        public AsyncDataTransfer(IAsyncDataTransferItem item)
        {
            _items = [item];
        }

        public IReadOnlyList<DataFormat> Formats => _items[0].Formats;

        public IReadOnlyList<IAsyncDataTransferItem> Items => _items;

        public void Dispose()
        {

        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public sealed class AsyncDataTransferItem : IAsyncDataTransferItem, IDataObject
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private readonly List<DataFormat> _formats;
        private readonly object _item;
        
        public AsyncDataTransferItem(object item, params DataFormat[] formats)
        {
            _formats = formats.ToList();
            _item = item;
        }

        public IReadOnlyList<DataFormat> Formats => _formats;

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

        public Task<object> TryGetRawAsync(DataFormat format)
        {
            if (_formats.Contains(format))
                return Task.FromResult(_item);
            return Task.FromResult<object>(null);
        }
    }
}
