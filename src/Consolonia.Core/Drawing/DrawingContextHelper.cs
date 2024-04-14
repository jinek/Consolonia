using System;
using Avalonia;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Drawing
{
    internal static class DrawingContextHelper
    {
        // ReSharper disable once UnusedMethodReturnValue.Global Can be used later
        public static bool ExecuteWithClipping(this Rect rect, Point place, Action action)
        {
            if (!rect.ContainsExclusive(place)) return false;

            action();
            return true;
        }

        /// <summary>
        /// use <see cref="Rect.ContainsExclusive"/>
        /// </summary>
        [Obsolete("Use Rect.ContainsExclusive instead", true)]
        public static bool ContainsAligned(this Rect rect, Point p)
        {
            throw new NotImplementedException("Remove this method");
        }

        public static bool IsEmpty(this Rect rect)
        {
            //todo: check if this implementation serves our needs. Avalonia uses == 0
            return rect.Width <= 0 || rect.Height <= 0;
        }

        public static bool IsTranslateOnly(this Matrix transform)
        {
            return transform.M11.IsNearlyEqual(1)
                   && transform.M22.IsNearlyEqual(1)
                   && transform.M12.IsNearlyEqual(0)
                   && transform.M21.IsNearlyEqual(0);
        }

        public static bool NoRotation(this Matrix transform)
        {
            return transform.M12 == 0 && transform.M21 == 0;
        }
    }
}