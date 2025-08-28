using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     A clipboard implementation for X11;
    /// </summary>
    internal class X11Clipboard : IClipboard
    {
        public Task ClearAsync()
        {
            Medo.X11.X11Clipboard.Clipboard.Clear();
            return Task.CompletedTask;
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
        }

        public async Task<object> GetDataAsync(string format)
        {
            if (string.Equals(format, "text", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(format, "unicodetext", StringComparison.OrdinalIgnoreCase))
                return await GetTextAsync();
            return null;
        }

        public Task<string[]> GetFormatsAsync()
        {
            return Task.FromResult(new[] { "text", "unicodetext" });
        }

        public Task<string> GetTextAsync()
        {
            string text = Medo.X11.X11Clipboard.Clipboard.GetText();
            return Task.FromResult(text);
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public Task SetTextAsync(string text)
        {
            Medo.X11.X11Clipboard.Clipboard.SetText(text ?? string.Empty);
            return Task.CompletedTask;
        }

        public Task<IDataObject> TryGetInProcessDataObjectAsync()
        {
            throw new NotImplementedException();
        }
    }
}