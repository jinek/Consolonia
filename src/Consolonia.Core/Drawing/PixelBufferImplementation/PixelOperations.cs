using System;
using Avalonia.Media;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    internal static class PixelOperations
    {
        private static int _factor = 32;

        /// <summary>
        /// Make color darker
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Shade(this Color color)
        {
            return color.Shade(Colors.Black);
        }

        /// <summary>
        /// Make color brighter
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Brighten(this Color color)
        {
            return color.Brighten(Colors.Black);
        }

        /// <summary>
        /// Make color less contrast with background color
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Color Shade(this Color foreground, Color background)
        {
            var foregroundAvg = (foreground.R + foreground.B + foreground.G) / 3;
            var backgroundAvg = (background.R + background.B + background.G) / 3;
            if (foregroundAvg > backgroundAvg || (foregroundAvg == backgroundAvg && foregroundAvg > 0))
            {
                // if color is lighter than background shading should make it more dark (less constrast)
                return Color.FromRgb((byte)Math.Max(0, foreground.R - _factor),
                                     (byte)Math.Max(0, foreground.G - _factor),
                                     (byte)Math.Max(0, foreground.B - _factor));

            }
            else
            {
                // if color is darker than background shading should make it more bright (less contrast)
                return Color.FromRgb((byte)Math.Min(Byte.MaxValue, foreground.R + _factor),
                                     (byte)Math.Min(Byte.MaxValue, foreground.G + _factor),
                                     (byte)Math.Min(Byte.MaxValue, foreground.B + _factor));

            }
        }

        /// <summary>
        /// Make color more constrast with background color
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public static Color Brighten(this Color foreground, Color background)
        {
            var foregroundAvg = (foreground.R + foreground.B + foreground.G) / 3;
            var backgroundAvg = (background.R + background.B + background.G) / 3;
            if (foregroundAvg > backgroundAvg || (foregroundAvg == backgroundAvg && foregroundAvg < Byte.MaxValue))
            {
                // if color is lighter than background brighten should make it more bright (more contrast)
                return Color.FromRgb((byte)Math.Min(Byte.MaxValue, foreground.R + _factor),
                                     (byte)Math.Min(Byte.MaxValue, foreground.G + _factor),
                                     (byte)Math.Min(Byte.MaxValue, foreground.B + _factor));
            }
            else
            {
                // if color is darker than background brigthen should make it more dark (more constrast)
                return Color.FromRgb((byte)Math.Max(0, foreground.R - _factor),
                                     (byte)Math.Max(0, foreground.G - _factor),
                                     (byte)Math.Max(0, foreground.B - _factor));
            }
        }
    }
}