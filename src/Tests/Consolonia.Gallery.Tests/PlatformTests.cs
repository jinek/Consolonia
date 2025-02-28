using System;
using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class PlatformTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            string[] expected = new[]
            {
                "✓.*IsConsole",
                $"{(OperatingSystem.IsWindows() ? '✓' : '✗')}.*IsWindows",
                $"{(OperatingSystem.IsLinux() ? '✓' : '✗')}.*IsLinux",
                $"{(OperatingSystem.IsMacOS() ? '✓' : '✗')}.*IsOSX"
            };

            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(expected);
        }
    }
}