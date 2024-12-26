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

        public void SetAttributes(InputLessDefaultNetConsole console, Color background, Color foreground,
            FontWeight? weight)
        {
            (ConsoleColor backgroundConsoleColor, EgaColorMode mode) = ConvertToConsoleColorMode(background);
            /* todo: bring back and below
             if (mode is not EgaColorMode.Colored)
                ConsoloniaPlatform.RaiseNotSupported(62143, background);*/

            (ConsoleColor foregroundConsoleColor, mode) = ConvertToConsoleColorMode(foreground);
            /*if (mode is not EgaColorMode.Colored)
                ConsoloniaPlatform.RaiseNotSupported(62144, foreground);*/

            Console.ForegroundColor = foregroundConsoleColor;
            Console.BackgroundColor = backgroundConsoleColor;
        }

        private static (ConsoleColor, EgaColorMode) ConvertToConsoleColorMode(Color color)
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
                ConsoleColor.DarkBlue or
                    ConsoleColor.DarkGreen or
                    ConsoleColor.DarkCyan or
                    ConsoleColor.DarkRed or
                    ConsoleColor.DarkMagenta or
                    ConsoleColor.DarkYellow or
                    ConsoleColor.DarkGray or
                    ConsoleColor.Black => ConsoleColor.Black,
                _ => ConsoleColor.Black
            };
        }

        private static Color ConvertToAvaloniaColor(ConsoleColor consoleColor)
        {
            (ConsoleColor _, (int R, int G, int B) rgb) = ConsoleColorMap.First(c => c.Color == consoleColor);
            return Color.FromRgb((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }
    }
}