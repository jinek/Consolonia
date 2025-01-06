using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Avalonia.Media;

namespace Consolonia.Core.Text
{
    /// <summary>
    ///     ANSI escape definitions and utility methods.
    /// </summary>
    public static class Esc
    {
        public const char Escape = '\u001b';
        public const string CSI = "\u001b[";
        public const string OSC = "\u001b]";

        // Control
        public const string EndOfText = "\u0003";
        public const string EndOfTransmission = "\u0004";

        // style modifiers
        public const string Reset = $"{CSI}0m";
        public const string Normal = $"{CSI}22m";
        public const string Bold = $"{CSI}1m";
        public const string Dim = $"{CSI}2m";

        // text decorations
        public const string Italic = $"{CSI}3m";
        public const string Underline = $"{CSI}4m";
        public const string Strikethrough = $"{CSI}9m";

        // screen buffer
        public const string EnableAlternateBuffer = $"{CSI}?1049h";
        public const string DisableAlternateBuffer = $"{CSI}?1049l";
        public const string ClearScreen = $"{CSI}2J";

        // cursor
        public const string HideCursor = $"{CSI}?25l";
        public const string ShowCursor = $"{CSI}?25h";

        // cursor shape
        public const string BlinkingBlockCursor = $"{CSI}1 q";
        public const string SteadyBlockCursor = $"{CSI}2 q";
        public const string BlinkingUnderlineCursor = $"{CSI}3 q";
        public const string SteadyUnderlineCursor = $"{CSI}4 q";
        public const string BlinkingBarCursor = $"{CSI}5 q";
        public const string SteadyBarCursor = $"{CSI}6 q";

        // bracketed 
        public const string EnableBracketedPasteMode = $"{CSI}?2004h";
        public const string DisableBracktedPasteMode = $"{CSI}?2004l";

        // mouse tracking
        public const string EnableMouseTracking = $"{CSI}?1000h";
        public const string DisableMouseTracking = $"{CSI}?1000l";
        public const string EnableMouseMotionTracking = $"{CSI}?1002h";
        public const string DisableMouseMotionTracking = $"{CSI}?1002l";
        public const string EnableExtendedMouseTracking = $"{CSI}?1006h";
        public const string DisableExtendedMouseTracking = $"{CSI}?1006l";

        // Cursor Keys input sequences
        public const string ApplicationCursorKeys = $"{CSI}?1h";
        public const string NormalCursorKeys = $"{CSI}?1l";

        // Keypad input sequences
        public const string ApplicationKeypad = "\u001b=";
        public const string NumericKeypad = "\u001b>";

        // Save/Restore cursor position
        public const string SaveCursorPosition = $"{CSI}s";
        public const string RestoreCursorPosition = $"{CSI}u";

        // input ansi key codes
        public const string UpKey = $"{CSI}A";
        public const string DownKey = $"{CSI}B";
        public const string RightKey = $"{CSI}C";
        public const string LeftKey = $"{CSI}D";
        public const string HomeKey = $"{CSI}H";
        public const string EndKey = $"{CSI}F";
        public const string PageUpKey = $"{CSI}5~";
        public const string PageDownKey = $"{CSI}6~";
        public const string InsertKey = $"{CSI}2~";
        public const string DeleteKey = $"{CSI}3~";
        public const string F1Key = "\u001bOP";
        public const string F2Key = "\u001bOQ";
        public const string F3Key = "\u001bOR";
        public const string F4Key = "\u001bOS";
        public const string F5Key = $"{CSI}15~";
        public const string F6Key = $"{CSI}17~";
        public const string F7Key = $"{CSI}18~";
        public const string F8Key = $"{CSI}19~";
        public const string F9Key = $"{CSI}20~";
        public const string F10Key = $"{CSI}21~";
        public const string F11Key = $"{CSI}23~";
        public const string F12Key = $"{CSI}24~";
        public const string ControlUpArrow = $"{CSI}1;5A";
        public const string ControlDownArrow = $"{CSI}1;5B";
        public const string ControlRightArrow = $"{CSI}1;5C";
        public const string ControlLeftArrow = $"{CSI}1;5D";

        // input ansi mouse codes
        public const string MouseEventPrefix = $"{CSI}<";

        // key modifier 
        public const string KeyModifierPrefix = $"{CSI}1;";

        // move cursor
        public static string MoveCursorUp(int n)
        {
            return $"{CSI}{n}A";
        }

        public static string MoveCursorDown(int n)
        {
            return $"{CSI}{n}B";
        }

        public static string MoveCursorRight(int n)
        {
            return $"{CSI}{n}C";
        }

        public static string MoveCursorLeft(int n)
        {
            return $"{CSI}{n}D";
        }

        public static string SetCursorPosition(int x, int y)
        {
            return $"{CSI}{y + 1};{x + 1}f";
        }

        public static string Foreground(object color)
        {
            return color switch
            {
                ConsoleColor consoleColor => Foreground(consoleColor),
                Color rgbColor => Foreground(rgbColor),
                _ => throw new ArgumentException("Invalid color type")
            };
        }

        public static string Foreground(ConsoleColor color)
        {
            int ansiCode = GetAnsiCode(color);
            return ansiCode < 8
                ?
                // Standard colors
                $"{CSI}{30 + ansiCode}m"
                :
                // Bright colors
                $"{CSI}{90 + (ansiCode - 8)}m";
        }

        public static string Background(object color)
        {
            return color switch
            {
                ConsoleColor consoleColor => Background(consoleColor),
                Color rgbColor => Background(rgbColor),
                _ => throw new ArgumentException("Invalid color type")
            };
        }

        public static string Background(ConsoleColor color)
        {
            int ansiCode = GetAnsiCode(color);
            return ansiCode < 8
                ?
                // Standard colors
                $"{CSI}{40 + ansiCode}m"
                :
                // Bright colors
                $"{CSI}{100 + (ansiCode - 8)}m";
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
            return $"{CSI}38;2;{red};{green};{blue}m";
        }

        public static string Background(Color color)
        {
            return Background(color.R, color.G, color.B);
        }

        public static string Background(byte red, byte green, byte blue)
        {
            return $"{CSI}48;2;{red};{green};{blue}m";
        }

        public static string SetWindowTitle(string title)
        {
            return $"{OSC}0;{title}\u0007";
        }

        public static bool TryParseMouse(string sequence, out AnsiMouseCodes codes, out short x, out short y, out bool buttonPressed, out int wheelDelta)
        {
            x = 0;
            y = 0;
            codes = default;
            buttonPressed = false;
            wheelDelta = 0;

            // if it's a mouse sequence
            if (!String.IsNullOrEmpty(sequence) &&
                sequence.StartsWith(MouseEventPrefix, false, CultureInfo.InvariantCulture) &&
                Char.ToLowerInvariant(sequence.Last()) == 'm')
            {
                var parts = sequence[MouseEventPrefix.Length..].Split(';').ToArray();
                if (parts.Length == 3)
                {
                    codes = Enum.Parse<AnsiMouseCodes>(parts[0]);
                    if (codes.HasFlag(AnsiMouseCodes.MouseWheel))
                    {
                        if (codes.HasFlag(AnsiMouseCodes.Button2))
                            wheelDelta = -1;
                        else
                            wheelDelta = 1;
                    }
                    x = short.Parse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture);
                    y = short.Parse(parts[2].TrimEnd('m','M'), NumberStyles.Integer, CultureInfo.InvariantCulture);
                    buttonPressed = sequence.Last() == 'M';
                    Debug.WriteLine(sequence);
                    return true;
                }
                throw new ArgumentException("Invalid mouse sequence");
            }
            return false;
        }
    }

    [Flags]
    public enum AnsiMouseCodes  
    {
        Button1 = 0x00,
        Button2 = 0x01,
        Button3 = 0x02,

        Shift = 0x04,
        Alt = 0x08,
        Control = 0x10,
        
        MouseMove = 0x20,
        MouseWheel = 0x40,
    }
}