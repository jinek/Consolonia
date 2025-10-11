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
        private IDataObject _dataObject;
        private string _text = string.Empty;

#pragma warning disable CA1822 // Mark members as static
        public async Task ClearAsync()
        {
            await Task.CompletedTask;
            _text = string.Empty;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public Task<object> GetDataAsync(string format)
        {
            return Task.FromResult(_dataObject?.Get(format));
        }

        public Task<string[]> GetFormatsAsync()
        {
            return Task.FromResult(new[] { "text", "unicodetext" });
        }

        public Task<string> GetTextAsync()
        {
            return Task.FromResult(_text);
        }

        public Task SetDataAsync(IAsyncDataTransfer dataTransfer)
        {
            throw new System.NotImplementedException();
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            _text = null;
            _dataObject = data;
            return Task.CompletedTask;
        }

        public Task SetTextAsync(string text)
        {
            _text = text;
            _dataObject = null;
            return Task.CompletedTask;
        }

        public Task<IAsyncDataTransfer> TryGetDataAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IAsyncDataTransfer> TryGetInProcessDataAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IDataObject> TryGetInProcessDataObjectAsync()
        {
            return Task.FromResult(_dataObject);
        }
    }
}