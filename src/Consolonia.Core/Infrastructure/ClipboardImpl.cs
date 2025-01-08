using System;
using System.Threading.Tasks;
using Avalonia.Input;
using TextCopy;

namespace Consolonia.Core.Infrastructure
{
    // TODO: Replace this with avalonia platform implementation
    //
    // The issue is that the current Avalonia Platform implementation is only easily accessible on win32 platform
    // for linux and mac it has a ton of dependencies on the entire rendering subsystem. 
    // This is a temporary solution to get the clipboard working, but it is not a good solution for linux as it relies 
    // on invoking a shell command to get the clipboard content, and doesn't support GetData/SetData/GetFormats
    internal class ClipboardImpl : Avalonia.Input.Platform.IClipboard
    {
#pragma warning disable CA1822 // Mark members as static
        public Task ClearAsync()
            => ClipboardService.SetTextAsync(string.Empty);

        public Task<object> GetDataAsync(string format)
        {
            throw new NotImplementedException();
        }

        public Task<string[]> GetFormatsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTextAsync()
            => ClipboardService.GetTextAsync();

        public Task SetDataObjectAsync(IDataObject data)
        {
            throw new NotImplementedException();
        }

        public Task SetTextAsync(string text)
            => ClipboardService.SetTextAsync(text ?? String.Empty);
    }
}
