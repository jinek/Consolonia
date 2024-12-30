using System;
using Avalonia.Media;
using Wcwidth;

namespace Consolonia.Core.Text
{
    /// <summary>
    ///     ANSI escape definitions and utility methods.
    /// </summary>
    internal static class Esc
    {
        // Control
        public const string EndOfText = "\u0003";
        public const string EndOfTransmission = "\u0004";

        // style modifiers
        public const string Reset = "\u001b[0m";
        public const string Normal = "\u001b[22m";
        public const string Bold = "\u001b[1m";
        public const string Dim = "\u001b[2m";

        // text decorations
        public const string Italic = "\u001b[3m";
        public const string Underline = "\u001b[4m";
        public const string Strikethrough = "\u001b[9m";

        // screen buffer
        public const string EnableAlternateBuffer = "\u001b[?1049h";
        public const string DisableAlternateBuffer = "\u001b[?1049l";
        public const string ClearScreen = "\u001b[2J";

        // cursor
        public const string HideCursor = "\u001b[?25l";
        public const string ShowCursor = "\u001b[?25h";

        // cursor shape
        public const string BlinkingBlockCursor = "\u001b[1 q";
        public const string SteadyBlockCursor = "\u001b[2 q";
        public const string BlinkingUnderlineCursor = "\u001b[3 q";
        public const string SteadyUnderlineCursor = "\u001b[4 q";
        public const string BlinkingBarCursor = "\u001b[5 q";
        public const string SteadyBarCursor = "\u001b[6 q";

        // move cursor
        public static string MoveCursorUp(int n)
        {
            return $"\u001b[{n}A";
        }

        public static string MoveCursorDown(int n)
        {
            return $"\u001b[{n}B";
        }

        public static string MoveCursorRight(int n)
        {
            return $"\u001b[{n}C";
        }

        public static string MoveCursorLeft(int n)
        {
            return $"\u001b[{n}D";
        }

        public static string SetCursorPosition(int x, int y)
        {
            return $"\u001b[{y + 1};{x + 1}f";
        }

        public static string Foreground(ConsoleColor color)
        {
            var ansiCode = GetAnsiCode(color);
            return ansiCode < 8
                ?
                // Standard colors
                $"\x1b[{(30 + ansiCode)}m"
                :
                // Bright colors
                $"\x1b[{(90 + (ansiCode - 8))}m";
        }

        public static string Background(ConsoleColor color)
        {
            var ansiCode = GetAnsiCode(color);
            return ansiCode < 8
                ?
            // Standard colors
                $"\x1b[{40 + ansiCode}m"
            :
            // Bright colors
                $"\x1b[{100 + (ansiCode - 8)}m";
        }

        public static int GetAnsiCode(ConsoleColor color)
        {
            return color switch
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
        }

        public static string Foreground(Color color)
        {
            return Foreground(color.R, color.G, color.B);
        }

        public static string Foreground(byte red, byte green, byte blue)
        {
            return $"\u001b[38;2;{red};{green};{blue}m";
        }

        public static string Background(Color color)
        {
            return Background(color.R, color.G, color.B);
        }

        public static string Background(byte red, byte green, byte blue)
        {
            return $"\u001b[48;2;{red};{green};{blue}m";
        }

        public static string SetWindowTitle(string title)
        {
            return $"\u001b]0;{title}\u0007";
        }
    }
}