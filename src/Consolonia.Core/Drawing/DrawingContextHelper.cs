using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Drawing
{
    internal static class DrawingContextHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this Rect rect)
        {
            //todo: check if this implementation serves our needs. Avalonia uses == 0
            return rect.Width <= 0 || rect.Height <= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(this PixelRect rect)
        {
            //todo: check if this implementation serves our needs. Avalonia uses == 0
            return rect.Width <= 0 || rect.Height <= 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PixelRect ToPixelRect(this Rect rect)
        {
            return new PixelRect((int)Math.Floor(rect.X),
                (int)Math.Floor(rect.Y),
                (int)Math.Round(rect.Width),
                (int)Math.Round(rect.Height));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PixelPoint ToPixelPoint(this Point point)
        {
            return new PixelPoint((int)Math.Floor(point.X),
                (int)Math.Floor(point.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTranslateOnly(this Matrix transform)
        {
            return transform.M11.IsNearlyEqual(1)
                   && transform.M22.IsNearlyEqual(1)
                   && transform.M12.IsNearlyEqual(0)
                   && transform.M21.IsNearlyEqual(0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NoRotation(this Matrix transform)
        {
            return transform is { M12: 0, M21: 0 };
        }
    }
}