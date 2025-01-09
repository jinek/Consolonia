using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     This clipboard only stores memory in the same process, so it is not useful for sharing data between processes.
    /// </summary>
    public class NaiveClipboard : IClipboard
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

        public async Task<string> GetTextAsync()
        {
            await Task.CompletedTask;
            return _text;
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public async Task SetTextAsync(string text)
        {
            await Task.CompletedTask;
            _text = text;
        }
    }
}