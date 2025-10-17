using System.Threading.Tasks;
using Consolonia.Core.Infrastructure;

namespace Consolonia.PlatformSupport.Clipboard
{
    /// <summary>
    ///     A clipboard implementation for X11;
    /// </summary>
    internal class X11Clipboard : ConsoleClipboard
    {
        public override async Task ClearAsync()
        {
            await base.ClearAsync();

            Medo.X11.X11Clipboard.Clipboard.Clear();
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