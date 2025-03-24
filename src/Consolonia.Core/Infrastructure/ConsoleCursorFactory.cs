using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Input;
using Avalonia.Platform;

namespace Consolonia.Core.Infrastructure
{
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    internal sealed class ConsoleCursorFactory : ICursorFactory, ICursorImpl
    {
        private readonly Dictionary<StandardCursorType, CursorImpl> standardCursors = new Dictionary<StandardCursorType, CursorImpl>();

        public ICursorImpl GetCursor(StandardCursorType cursorType)
        {
            lock (standardCursors)
            {
                if (standardCursors.TryGetValue(cursorType, out var cursor))
                    return cursor;
                cursor = new CursorImpl(cursorType);
                standardCursors.Add(cursorType, cursor);
                return cursor;
            }
        }

        public ICursorImpl CreateCursor(IBitmapImpl cursor, PixelPoint hotSpot)
        {
            throw new NotSupportedException($"Consolonia doesn't support bitmap based cursors");
        }

        public void Dispose()
        {
        }
    }

    public sealed class CursorImpl : ICursorImpl
    {
        public CursorImpl(StandardCursorType cursorType)
        {
            CursorType = cursorType;
        }

        public StandardCursorType CursorType { get; init; }

        public void Dispose()
        {
        }
    }

}