using System;
using Avalonia.Media;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    internal static class PixelOperations
    {
        private const int ColorAdjustment = 32;

        /// <summary>
        ///     Make color darker
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Shade(this Color color)
        {
            return color.Shade(Colors.Black);
        }

        /// <summary>
        ///     Make color brighter
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Brighten(this Color color)
        {
            return color.Brighten(Colors.Black);
        }

        /// <summary>
        ///     Make color less contrast with background color
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Color Shade(this Color foreground, Color background)
        {
            int foregroundBrightness = GetPerceivedBrightness(foreground);
            int backgroundBrightness = GetPerceivedBrightness(background);
            if (foregroundBrightness > backgroundBrightness ||
                foregroundBrightness == backgroundBrightness && foregroundBrightness > 0)
                // if color is lighter than background shading should make it more dark (less contrast)
                return Color.FromRgb((byte)Math.Max(0, foreground.R - ColorAdjustment),
                    (byte)Math.Max(0, foreground.G - ColorAdjustment),
                    (byte)Math.Max(0, foreground.B - ColorAdjustment));
            // if color is darker than background shading should make it more bright (less contrast)
            return Color.FromRgb((byte)Math.Min(byte.MaxValue, foreground.R + ColorAdjustment),
                (byte)Math.Min(byte.MaxValue, foreground.G + ColorAdjustment),
                (byte)Math.Min(byte.MaxValue, foreground.B + ColorAdjustment));
        }

        /// <summary>
        ///     Make color more contrast with background color
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Color Brighten(this Color foreground, Color background)
        {
            int foregroundBrightness = GetPerceivedBrightness(foreground);
            int backgroundBrightness = GetPerceivedBrightness(background);
            if (foregroundBrightness > backgroundBrightness || foregroundBrightness == backgroundBrightness &&
                foregroundBrightness < byte.MaxValue)
                // if color is lighter than background brighten should make it more bright (more contrast)
                return Color.FromRgb((byte)Math.Min(byte.MaxValue, foreground.R + ColorAdjustment),
                    (byte)Math.Min(byte.MaxValue, foreground.G + ColorAdjustment),
                    (byte)Math.Min(byte.MaxValue, foreground.B + ColorAdjustment));
            // if color is darker than background brighten should make it more dark (more contrast)
            return Color.FromRgb((byte)Math.Max(0, foreground.R - ColorAdjustment),
                (byte)Math.Max(0, foreground.G - ColorAdjustment),
                (byte)Math.Max(0, foreground.B - ColorAdjustment));
        }

        private static int GetPerceivedBrightness(Color color)
        {
            return (int)(0.299 * color.R +
                         0.587 * color.G +
                         0.114 * color.B);
        }
    }
}