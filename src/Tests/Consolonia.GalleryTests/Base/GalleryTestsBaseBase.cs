using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Gallery;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests.Base
{
    internal abstract class GalleryTestsBaseBase : ConsoloniaAppTestBase<App>
    {
        protected GalleryTestsBaseBase(PixelBufferSize size = default) : base(size.IsEmpty
            ? new PixelBufferSize(80, 40)
            : size)
        {
            Args = new string[2];
            Args[1] = GetType().Name[..^5];
        }

        [Test]
        public async Task SingleTest()
        {
            await UITest.KeyInput(Key.Tab, Key.Tab);
            await PerformSingleTest();
        }

        protected abstract Task PerformSingleTest();
    }
}