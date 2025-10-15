using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     This clipboard only stores memory in the same process, so it is not useful for sharing data between processes.
    /// </summary>
    public class ConsoleClipboard : IClipboardImpl
    {
        private IAsyncDataTransfer _asyncDataTransfer = new AsyncDataTransfer();
        private string _text;

        public async Task SetDataAsync(IAsyncDataTransfer dataTransfer)
        {
            _asyncDataTransfer = dataTransfer;
            var text = await dataTransfer.TryGetTextAsync();
            if (text != null)
            {
                // promote text into platform clipboard
                await SetTextAsync(text);
            }
        }

        public async Task<IAsyncDataTransfer> TryGetDataAsync()
        {
            List<IAsyncDataTransferItem> items = _asyncDataTransfer.Items.Where(i => !i.Contains(DataFormat.Text)).ToList();
            var text = await GetTextAsync();
            if (text != null)
                items.Add(new AsyncDataTransferItem(text, DataFormat.Text));
            return new AsyncDataTransfer(items);
        }

        public async virtual Task ClearAsync()
        {
            _asyncDataTransfer = new AsyncDataTransfer();
            _text = null;
        }

        public virtual async Task SetTextAsync(string text)
            => _text = text;

        public virtual async Task<string> GetTextAsync()
            => _text;
    }
}