using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaScreen : IScreenImpl
    {
        private readonly Screen[] _screens =
            [new(0.0, new PixelRect(0, 0, 1920, 1080), new PixelRect(0, 0, 1920, 1080), true)];

        public int ScreenCount => 1;

        public IReadOnlyList<Screen> AllScreens => _screens.AsReadOnly();

        public Action Changed { get; set; }

        public Task<bool> RequestScreenDetails()
        {
            return Task.FromResult(true);
        }

        public Screen ScreenFromPoint(PixelPoint point)
        {
            return _screens[0];
        }

        public Screen ScreenFromRect(PixelRect rect)
        {
            return _screens[0];
        }

        public Screen ScreenFromTopLevel(ITopLevelImpl topLevel)
        {
            return _screens[0];
        }

        public Screen ScreenFromWindow(IWindowBaseImpl window)
        {
            return _screens[0];
        }
    }
}