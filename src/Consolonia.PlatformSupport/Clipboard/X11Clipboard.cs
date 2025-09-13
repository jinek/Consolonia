using System;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     A clipboard implementation for X11;
    /// </summary>
    internal class X11Clipboard : IClipboardBase
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