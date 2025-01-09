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
        public X11Clipboard()
        {
        }

        public async Task ClearAsync()
        {
            await Task.CompletedTask;
            Medo.X11.X11Clipboard.Clipboard.Clear();
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
            return Medo.X11.X11Clipboard.Clipboard.GetText();
        }

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public async Task SetTextAsync(string text)
        {
            await Task.CompletedTask;

            Medo.X11.X11Clipboard.Clipboard.SetText(text);
        }
    }
}
