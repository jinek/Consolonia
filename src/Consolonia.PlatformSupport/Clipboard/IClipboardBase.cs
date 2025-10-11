using System;
using System.Threading.Tasks;
using Avalonia.Input.Platform;

namespace Consolonia.PlatformSupport.Clipboard
{
    internal interface IClipboardBase : IClipboard
    {
        async Task<object> IClipboard.GetDataAsync(string format)
        {
            if (string.Equals(format, "text", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(format, "unicodetext", StringComparison.OrdinalIgnoreCase))
                return await GetTextAsync();
            return null;
        }
    }
}