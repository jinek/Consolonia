using System;
using Avalonia;

namespace Consolonia.Core.Drawing
{
    internal static class DrawingContextHelper
    {
        public static void ExecuteWithClipping(this Rect rect,Point place,Action action)
        {
            if (ContainsAligned(rect, place))
                action();
        }

        public static bool ContainsAligned(this Rect rect, Point p)
        {
            (double x, double y) = p;
            return x >= rect.X && x < rect.X + rect.Width &&
                   y >= rect.Y && y < rect.Y + rect.Height;
        }
    }
}