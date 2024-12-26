using System;
using System.Linq;
using System.Text;
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
            if (mode is not EgaColorMode.Colored)
                ConsoloniaPlatform.RaiseNotSupported(62144, foreground);
            

            (ConsoleColor foregroundConsoleColor, mode) = ConvertToConsoleColorMode(foreground);
            //todo: if mode is transparent, don't print foreground. if shaded - shade it

            var sb = new StringBuilder();

            // Append ANSI escape sequence for background color
            sb.Append(GetAnsiCode(backgroundConsoleColor, true));

            // Append ANSI escape sequence for foreground color
            sb.Append(GetAnsiCode(foregroundConsoleColor, false));
            console.WriteText(sb.ToString());
            return;

            // Function to map ConsoleColor to ANSI code
            static string GetAnsiCode(ConsoleColor color, bool isBackground)
            {
                int ansiCode = color switch
                {
                    ConsoleColor.Black => 0,
                    ConsoleColor.DarkRed => 1,
                    ConsoleColor.DarkGreen => 2,
                    ConsoleColor.DarkYellow => 3,
                    ConsoleColor.DarkBlue => 4,
                    ConsoleColor.DarkMagenta => 5,
                    ConsoleColor.DarkCyan => 6,
                    ConsoleColor.Gray => 7,
                    ConsoleColor.DarkGray => 8,
                    ConsoleColor.Red => 9,
                    ConsoleColor.Green => 10,
                    ConsoleColor.Yellow => 11,
                    ConsoleColor.Blue => 12,
                    ConsoleColor.Magenta => 13,
                    ConsoleColor.Cyan => 14,
                    ConsoleColor.White => 15,
                    _ => 7 // Default to white if unknown
                };

                return ansiCode < 8
                    ?
                    // Standard colors
                    $"\x1b[{(isBackground ? 40 + ansiCode : 30 + ansiCode)}m"
                    :
                    // Bright colors
                    $"\x1b[{(isBackground ? 100 + (ansiCode - 8) : 90 + (ansiCode - 8))}m";
            }
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