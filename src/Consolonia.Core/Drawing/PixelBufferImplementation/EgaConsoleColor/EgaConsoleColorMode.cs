using System;
using System.Linq;
using Avalonia.Media;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor
{
    public class EgaConsoleColorMode : IConsoleColorMode
    {
        private static readonly (ConsoleColor Color, (int R, int G, int B) Rgb)[] ConsoleColorMap =
        [
            (ConsoleColor.Black, (0, 0, 0)),
            (ConsoleColor.DarkBlue, (0, 0, 128)),
            (ConsoleColor.DarkGreen, (0, 128, 0)),
            (ConsoleColor.DarkCyan, (0, 128, 128)),
            (ConsoleColor.DarkRed, (128, 0, 0)),
            (ConsoleColor.DarkMagenta, (128, 0, 128)),
            (ConsoleColor.DarkYellow, (128, 128, 0)),
            (ConsoleColor.Gray, (192, 192, 192)),
            (ConsoleColor.DarkGray, (128, 128, 128)),
            (ConsoleColor.Blue, (0, 0, 255)),
            (ConsoleColor.Green, (0, 255, 0)),
            (ConsoleColor.Cyan, (0, 255, 255)),
            (ConsoleColor.Red, (255, 0, 0)),
            (ConsoleColor.Magenta, (255, 0, 255)),
            (ConsoleColor.Yellow, (255, 255, 0)),
            (ConsoleColor.White, (255, 255, 255))
        ];

        public Color Blend(Color color1, Color color2)
        {
            (ConsoleColor consoleColor1, EgaColorMode mode1) = ConvertToConsoleColorMode(color1);
            (ConsoleColor consoleColor2, EgaColorMode mode2) = ConvertToConsoleColorMode(color2);

            switch (mode2)
            {
                case EgaColorMode.Transparent:
                    return color1;

                case EgaColorMode.Shaded when mode1 == EgaColorMode.Shaded:
                {
                    ConsoleColor doubleShadedColor = Shade(Shade(consoleColor1));
                    return ConvertToAvaloniaColor(doubleShadedColor);
                }
                case EgaColorMode.Shaded:
                {
                    ConsoleColor shadedColor = Shade(consoleColor1);
                    return ConvertToAvaloniaColor(shadedColor);
                }
                case EgaColorMode.Colored:
                default:
                    return ConvertToAvaloniaColor(consoleColor2);
            }
        }

        public (object background, object foreground) MapColors(Color background, Color foreground, FontWeight? weight)
        {
            (ConsoleColor backgroundConsoleColor, EgaColorMode mode) = ConvertToConsoleColorMode(background);
            if (mode is not EgaColorMode.Colored)
                ConsoloniaPlatform.RaiseNotSupported(62144, foreground);

            (ConsoleColor foregroundConsoleColor, _) = ConvertToConsoleColorMode(foreground);
            //todo: if mode is transparent, don't print foreground. if shaded - shade it

            return (backgroundConsoleColor, foregroundConsoleColor);
        }

        public static (ConsoleColor, EgaColorMode) ConvertToConsoleColorMode(Color color)
        {
            ConsoleColor consoleColor = MapToConsoleColor(color);

            EgaColorMode mode = color.A switch
            {
                <= 63 => EgaColorMode.Transparent,
                <= 191 => EgaColorMode.Shaded,
                _ => EgaColorMode.Colored
            };

            return (consoleColor, mode);
        }

        private static ConsoleColor MapToConsoleColor(Color color)
        {
            int r = color.R, g = color.G, b = color.B;

            // Find the nearest ConsoleColor by RGB distance
            return ConsoleColorMap
                .OrderBy(c => Math.Pow(c.Rgb.R - r, 2) + Math.Pow(c.Rgb.G - g, 2) + Math.Pow(c.Rgb.B - b, 2))
                .First().Color;
        }

        private static ConsoleColor Shade(ConsoleColor color)
        {
            return color switch
            {
                ConsoleColor.White => ConsoleColor.Gray,
                ConsoleColor.Gray => ConsoleColor.DarkGray,
                ConsoleColor.Blue => ConsoleColor.DarkBlue,
                ConsoleColor.Green => ConsoleColor.DarkGreen,
                ConsoleColor.Cyan => ConsoleColor.DarkCyan,
                ConsoleColor.Red => ConsoleColor.DarkRed,
                ConsoleColor.Magenta => ConsoleColor.DarkMagenta,
                ConsoleColor.Yellow => ConsoleColor.DarkYellow,
                _ => ConsoleColor.Black
            };
        }

        public static Color ConvertToAvaloniaColor(ConsoleColor consoleColor,
            EgaColorMode mode = EgaColorMode.Colored)
        {
            switch (mode)
            {
                case EgaColorMode.Transparent:
                    return Color.FromArgb(0, 0, 0, 0);
                case EgaColorMode.Shaded:
                    return Color.FromArgb(127, 0, 0, 0);
                case EgaColorMode.Colored:
                    (ConsoleColor _, (int R, int G, int B) rgb) = ConsoleColorMap.First(c => c.Color == consoleColor);
                    return Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}