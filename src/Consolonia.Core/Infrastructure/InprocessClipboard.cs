using System;
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
        private string _text = string.Empty;
#pragma warning disable CA1822 // Mark members as static
        public async Task ClearAsync()
        {
            await Task.CompletedTask;
            _text = string.Empty;
        }

        public Task<object> GetDataAsync(string format)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetFormatsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTextAsync()
        {
            return Task.FromResult(_text);
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public Task SetTextAsync(string text)
        {
            _text = text;
            return Task.CompletedTask;
        }
    }
}