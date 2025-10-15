using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Consolonia.Core.Infrastructure;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     A clipboard implementation for X11;
    /// </summary>
    internal class X11Clipboard : ConsoleClipboard
    {
        public override Task ClearAsync()
        {
            Medo.X11.X11Clipboard.Clipboard.Clear();
            return Task.CompletedTask;
        }

        public override Task<string> GetTextAsync()
        {
            string text = Medo.X11.X11Clipboard.Clipboard.GetText();
            return Task.FromResult(text);
        }

        public override Task SetTextAsync(string text)
        {
            Medo.X11.X11Clipboard.Clipboard.SetText(text ?? string.Empty);
            return Task.CompletedTask;
        }
    }
}