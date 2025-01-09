using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.Core.Infrastructure
{
    // TODO: Replace this with avalonia platform implementation
    internal class NaiveClipboard : IClipboard
    {
        private string _text = String.Empty;

#pragma warning disable CA1822 // Mark members as static
        public async Task ClearAsync()
        {
            _text = String.Empty;
        }

        public async Task<object> GetDataAsync(string format)
        {
            throw new NotImplementedException();
        }

        public async Task<string[]> GetFormatsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTextAsync()
        {
            return _text;
        }

        public async Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public async Task SetTextAsync(string text)
        {
            _text = text;
        }
    }
}