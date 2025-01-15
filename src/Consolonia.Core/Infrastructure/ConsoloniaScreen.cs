using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaScreen : IScreenImpl
    {
        private Screen[] _screens = [new Screen(0.0, new PixelRect(0, 0, 1920, 1080), new PixelRect(0, 0, 1920, 1080), true)];

        public ConsoloniaScreen()
        {
        }

        public int ScreenCount => 1;

        public IReadOnlyList<Screen> AllScreens => _screens.AsReadOnly();

        public Action Changed { get; set; }

        public Task<bool> RequestScreenDetails() => Task.FromResult(true);

        public Screen ScreenFromPoint(PixelPoint point) => _screens[0];

        public Screen ScreenFromRect(PixelRect rect) => _screens[0];

        public Screen ScreenFromTopLevel(ITopLevelImpl topLevel) => _screens[0];

        public Screen ScreenFromWindow(IWindowBaseImpl window) => _screens[0];
    }
}