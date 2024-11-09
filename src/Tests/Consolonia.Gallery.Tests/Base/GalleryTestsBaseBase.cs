using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Gallery.View;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests.Base
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

        [OneTimeSetUp]
        public async Task Setup()
        {
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var controlsListView =
                    (ControlsListView)
                    ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime)!
                    .MainWindow!;
                controlsListView!.ChangeTo(Args);
            });

            await UITest.WaitRendered();
        }
    }
}