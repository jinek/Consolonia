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
    public class ConsoleClipboard : IClipboardImpl, IDisposable
    {
        private IAsyncDataTransfer _asyncDataTransfer = new AsyncDataTransfer();
        private bool _disposed;
        private string _text;

        public async Task SetDataAsync(IAsyncDataTransfer dataTransfer)
        {
            _asyncDataTransfer = dataTransfer;
            string text = await dataTransfer.TryGetTextAsync();
            if (text != null)
                // promote text into platform clipboard
                await SetTextAsync(text);
        }

        public async Task<IAsyncDataTransfer> TryGetDataAsync()
        {
            List<IAsyncDataTransferItem> items = _asyncDataTransfer.Items.Where(i => !i.Contains(DataFormat.Text))
                .ToList();
            string text = await GetTextAsync();
            if (text != null)
                items.Add(new AsyncDataTransferItem(text, DataFormat.Text));
            return new AsyncDataTransfer(items);
        }

        public virtual Task ClearAsync()
        {
            _asyncDataTransfer = new AsyncDataTransfer();
            _text = null;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual Task SetTextAsync(string text)
        {
            _text = text;
            return Task.CompletedTask;
        }

        public virtual Task<string> GetTextAsync()
        {
            return Task.FromResult(_text);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _asyncDataTransfer?.Dispose();
                    _asyncDataTransfer = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }
    }
}