using Avalonia.Media;

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
    }
}