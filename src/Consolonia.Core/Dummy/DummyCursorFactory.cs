using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;

namespace Consolonia.Core.Dummy
{
    public class asdf : ICursorImpl
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
    internal class DummyCursorFactory : ICursorFactory, ICursorImpl
    {
        public ICursorImpl GetCursor(StandardCursorType cursorType)
        {
            return this;
        }

        public ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot)
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}