using System;
using System.Threading.Tasks;
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
        public async Task InprocClipboardTest()
        {
            IClipboard clipboard = new InprocessClipboard();

            string text = await clipboard.GetTextAsync();

            await clipboard.SetTextAsync("Hello, World!");
            var hello = await clipboard.GetTextAsync();
            Assert.AreEqual("Hello, World!", hello);
            await clipboard.ClearAsync();
            Assert.AreEqual(String.Empty, await clipboard.GetTextAsync());

            // restore clipboard
            await clipboard.SetTextAsync(text);
        }

        // NOTE: This can mess up your clipboard state !
        [Test]
        public async Task Win32ClipboardTest()
        {
            IClipboard clipboard = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT => new Win32Clipboard(),
                PlatformID.Unix => PlatformSupportExtensions.IsWslPlatform() ? new WslClipboard() : new X11Clipboard(),
                PlatformID.MacOSX => new MacClipboard(),
                _ => new InprocessClipboard()
            };

            string text = await clipboard.GetTextAsync();

            await clipboard.SetTextAsync("Hello, World!");
            var hello = await clipboard.GetTextAsync();
            Assert.AreEqual("Hello, World!", hello);
            await clipboard.ClearAsync();
            Assert.AreEqual(String.Empty, await clipboard.GetTextAsync());

            // restore clipboard
            await clipboard.SetTextAsync(text);
        }
    }
}