using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Gallery.View;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests.Base
{
    internal class GalleryTestsBaseBase : ConsoloniaAppTestBase<App>
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
                var mainWindow =
                    (MainWindow)
                    ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime)!
                    .MainWindow!;
                var controlsListView = mainWindow.FindDescendantOfType<ControlsListView>();
                controlsListView!.ChangeTo(Args);
            });

            await UITest.WaitRendered();
        }
    }
}