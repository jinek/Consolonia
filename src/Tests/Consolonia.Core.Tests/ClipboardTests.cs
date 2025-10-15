using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport.Clipboard;
using NUnit.Framework;

namespace Consolonia.Core.Tests
{
#pragma warning disable CA1416 // Validate platform compatibility
    [TestFixture]
    public class ClipboardTests
    {
        [Test]
        public async Task ConsoleClipboardTest()
        {
            ConsoleClipboard clipboard = new ConsoleClipboard();

            await clipboard.SetDataAsync(new AsyncDataTransfer(new AsyncDataTransferItem("Hello, World!", DataFormat.Text)));
            var data = await clipboard.TryGetDataAsync();
            var text = await data.TryGetTextAsync();
            Assert.AreEqual("Hello, World!", text);

            await clipboard.ClearAsync();
            data = await clipboard.TryGetDataAsync();
            text = await data.TryGetTextAsync();
            Assert.IsNull(text);
        }


        [Test]
        [Ignore("This doesn't run on server because X11 no there")]
        public async Task PlatformClipboardTest()
        {
            ConsoleClipboard clipboard = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => new Win32Clipboard(),
                PlatformID.Unix => PlatformSupportExtensions.IsWslPlatform() ? new WslClipboard() : new X11Clipboard(),
                PlatformID.MacOSX => new MacClipboard(),
                _ => new ConsoleClipboard()
            };

            string origText = await clipboard.GetTextAsync();

            await clipboard.SetDataAsync(new AsyncDataTransfer(new AsyncDataTransferItem("Hello, World!", DataFormat.Text)));
            var data = await clipboard.TryGetDataAsync();
            var text = await data.TryGetTextAsync();
            Assert.AreEqual("Hello, World!", text);

            await clipboard.ClearAsync();
            data = await clipboard.TryGetDataAsync();
            text = await data.TryGetTextAsync();
            Assert.IsTrue(String.IsNullOrEmpty(text));

            // restore clipboard
            await clipboard.SetTextAsync(origText);
        }

        // NOTE: This can mess up your clipboard state !
    }
}
