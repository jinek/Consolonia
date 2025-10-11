using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     This clipboard only stores memory in the same process, so it is not useful for sharing data between processes.
    /// </summary>
    public class InprocessClipboard : IClipboard
    {
        private IAsyncDataTransfer _dataTransferAsync;

#pragma warning disable CA1822 // Mark members as static
        public Task ClearAsync()
        {
            _dataTransferAsync = null;
            return Task.CompletedTask;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<object> GetDataAsync(string format)
        {
            // legacy support
            var item = _dataTransferAsync?.Items?.Count > 0 ? _dataTransferAsync.Items[0] : null;
            if (item == null)
                return null;

            var fm = item.Formats.FirstOrDefault(f => f.Identifier == format);
            if (fm != null)
            {
                return await item.TryGetRawAsync(fm);
            }
            return null;
        }

        public Task<string[]> GetFormatsAsync()
        {
            // legacy support
            return Task.FromResult(_dataTransferAsync.Items.FirstOrDefault()?
                                        .Formats.Select(f => f.Identifier)
                                        .ToArray());
        }

        public async Task<string> GetTextAsync()
        {
            // legacy support
            var item = _dataTransferAsync?.Items?.Count > 0 ? _dataTransferAsync.Items[0] : null;
            if (item == null)
                return null;

            return (string)await item.TryGetRawAsync(DataFormat.Text);
        }

        public Task SetTextAsync(string text)
        {
            return SetDataAsync(new AsyncDataTransfer(new AsyncDataTransferItem(text, DataFormat.Text)));
        }

        public Task SetDataAsync(IAsyncDataTransfer dataTransfer)
        {
            _dataTransferAsync = dataTransfer;
            return Task.CompletedTask;
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public Task SetDataObjectAsync(IDataObject data)
        {
            return SetDataAsync(new AsyncDataTransfer(new AsyncDataTransferItem(data.GetText(), DataFormat.Text)));
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public Task<IAsyncDataTransfer> TryGetDataAsync()
        {
            return Task.FromResult(_dataTransferAsync);
        }

        public Task<IAsyncDataTransfer> TryGetInProcessDataAsync()
        {
            return Task.FromResult(_dataTransferAsync);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        public Task<IDataObject> TryGetInProcessDataObjectAsync()
        {
            return Task.FromResult(_dataTransferAsync as IDataObject);
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}