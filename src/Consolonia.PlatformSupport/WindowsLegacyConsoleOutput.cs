#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using static Vanara.PInvoke.Kernel32;

namespace Consolonia.PlatformSupport
{

    internal class WindowsLegacyConsoleOutput : IConsoleOutput
    {
        private WindowsConsoleBuffer _originalBuffer;
        private WindowsConsoleBuffer _consoleBuffer;
        private PixelBufferSize _size;

        public PixelBufferSize Size
        {
            get => _size;
            set
            {
                lock (_consoleBuffer.BufferHandle)
                {
                    GetConsoleScreenBufferInfo(_consoleBuffer.BufferHandle, out var info);
                    int windowWidth = info.srWindow.Right - info.srWindow.Left + 1;
                    int windowHeight = info.srWindow.Bottom - info.srWindow.Top + 1;
                    // Debug.WriteLine($"ScreenBuffer: {info.dwSize.X}x{info.dwSize.Y} NewValue: {value.Width}x{value.Height} Window: {windowWidth}x{windowHeight}");

                    if (value.Height > windowHeight)
                    {
                        // we need to adjust the buffer size to match to make scroll bars go away.
                        _size = new PixelBufferSize((ushort)windowWidth, (ushort)windowHeight);
                        SetConsoleScreenBufferSize(_consoleBuffer.BufferHandle, new COORD((short)_size.Width, (short)_size.Height));
                    }
                    else
                    {
                        _size = value;
                    }
                    ClearScreen();
                }
            }
        }

        public bool CaretVisible
        {
            get
            {
                if (!GetConsoleCursorInfo(_consoleBuffer.BufferHandle, out var info))
                {
                    throw GetLastError().GetException();
                }
                return info.bVisible;
            }
        }

        public bool SupportsComplexEmoji => false;

        public WindowsLegacyConsoleOutput()
        {

        }


        public void PrepareConsole()
        {
            Console.OutputEncoding = Encoding.Unicode;
            _originalBuffer = WindowsConsoleBuffer.GetCurrentConsoleScreenBuffer();

            // create secondary buffer
            _consoleBuffer = WindowsConsoleBuffer.Create();
            _consoleBuffer.SetAsActiveBuffer();

            lock (_consoleBuffer.BufferHandle)
            {
                if (!SetConsoleCP(65001) || !SetConsoleOutputCP(65001))
                {
                    throw GetLastError().GetException();
                }

                // set console mode
                if (!SetConsoleMode(_consoleBuffer.BufferHandle, CONSOLE_OUTPUT_MODE.DISABLE_NEWLINE_AUTO_RETURN |
                                        CONSOLE_OUTPUT_MODE.ENABLE_LVB_GRID_WORLDWIDE))
                {
                    throw GetLastError().GetException();
                }
            }
        }

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style, FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            var consoleColorMode = AvaloniaLocator.Current.GetRequiredService<IConsoleColorMode>();

            var (b, f) = consoleColorMode.MapColors(background, foreground, weight);

            if (b is not ConsoleColor backgroundConsoleColor)
                throw new InvalidCastException("Background color must be ConsoleColor");

            if (f is not ConsoleColor foregroundConsoleColor)
                throw new InvalidCastException("Foreground color must be ConsoleColor");

            CHARACTER_ATTRIBUTE backgroundAttributes = backgroundConsoleColor switch
            {
                ConsoleColor.Black => (CHARACTER_ATTRIBUTE)0,
                ConsoleColor.DarkRed => CHARACTER_ATTRIBUTE.BACKGROUND_RED,
                ConsoleColor.DarkGreen => CHARACTER_ATTRIBUTE.BACKGROUND_GREEN,
                ConsoleColor.DarkBlue => CHARACTER_ATTRIBUTE.BACKGROUND_BLUE,
                ConsoleColor.DarkYellow => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_GREEN,
                ConsoleColor.DarkMagenta => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE,
                ConsoleColor.DarkCyan => CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE,
                ConsoleColor.DarkGray => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE,
                ConsoleColor.Gray => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.Red => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.Green => CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.Blue => CHARACTER_ATTRIBUTE.BACKGROUND_BLUE | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.Yellow => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.Magenta => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.Cyan => CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                ConsoleColor.White => CHARACTER_ATTRIBUTE.BACKGROUND_RED | CHARACTER_ATTRIBUTE.BACKGROUND_GREEN | CHARACTER_ATTRIBUTE.BACKGROUND_BLUE | CHARACTER_ATTRIBUTE.BACKGROUND_INTENSITY,
                _ => throw new ArgumentOutOfRangeException(nameof(background))
            };

            CHARACTER_ATTRIBUTE foregroundAttributes = foregroundConsoleColor switch
            {
                ConsoleColor.Black => (CHARACTER_ATTRIBUTE)0,
                ConsoleColor.DarkRed => CHARACTER_ATTRIBUTE.FOREGROUND_RED,
                ConsoleColor.DarkGreen => CHARACTER_ATTRIBUTE.FOREGROUND_GREEN,
                ConsoleColor.DarkBlue => CHARACTER_ATTRIBUTE.FOREGROUND_BLUE,
                ConsoleColor.DarkYellow => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_GREEN,
                ConsoleColor.DarkMagenta => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE,
                ConsoleColor.DarkCyan => CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE,
                ConsoleColor.DarkGray => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE,
                ConsoleColor.Gray => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.Red => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.Green => CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.Blue => CHARACTER_ATTRIBUTE.FOREGROUND_BLUE | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.Yellow => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.Magenta => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.Cyan => CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                ConsoleColor.White => CHARACTER_ATTRIBUTE.FOREGROUND_RED | CHARACTER_ATTRIBUTE.FOREGROUND_GREEN | CHARACTER_ATTRIBUTE.FOREGROUND_BLUE | CHARACTER_ATTRIBUTE.FOREGROUND_INTENSITY,
                _ => throw new ArgumentOutOfRangeException(nameof(foreground))
            };
            var attributes = foregroundAttributes | backgroundAttributes;
            if (textDecoration == TextDecorationLocation.Underline)
                attributes |= CHARACTER_ATTRIBUTE.COMMON_LVB_UNDERSCORE;

            _consoleBuffer.DrawString(str, (short)bufferPoint.X, (short)bufferPoint.Y, attributes: attributes);
        }

        public void ClearScreen()
        {
            _consoleBuffer.Clear();
        }

        public void RestoreConsole()
        {
            _originalBuffer.SetAsActiveBuffer();
        }

        public void SetTitle(string title)
            => Console.Title = title;

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            lock (_consoleBuffer.BufferHandle)
            {
                SetConsoleCursorPosition(_consoleBuffer.BufferHandle, new COORD(bufferPoint.X, bufferPoint.Y));
            }
        }

        public PixelBufferCoordinate GetCaretPosition()
        {
            lock (_consoleBuffer.BufferHandle)
            {
                if (!GetConsoleScreenBufferInfo(_consoleBuffer.BufferHandle, out var info))
                {
                    throw GetLastError().GetException();
                }
                return new PixelBufferCoordinate((ushort)info.dwCursorPosition.X, (ushort)info.dwCursorPosition.Y);
            }
        }

        public void SetCaretStyle(CaretStyle caretStyle)
        {
            lock (_consoleBuffer.BufferHandle)
            {

                if (!GetConsoleCursorInfo(_consoleBuffer.BufferHandle, out var cursorInfo))
                {
                    throw GetLastError().GetException();
                }
                cursorInfo.dwSize = caretStyle switch
                {
                    CaretStyle.SteadyBlock => 100,
                    CaretStyle.BlinkingBlock => 100,
                    CaretStyle.SteadyUnderline => 1,
                    CaretStyle.BlinkingUnderline => 1,
                    CaretStyle.SteadyBar => 50,
                    CaretStyle.BlinkingBar => 50,
                    _ => throw new ArgumentOutOfRangeException(nameof(caretStyle))
                };

                if (!SetConsoleCursorInfo(_consoleBuffer.BufferHandle, cursorInfo))
                {
                    throw GetLastError().GetException();
                }
            }
        }

        public void HideCaret()
        {
            lock (_consoleBuffer.BufferHandle)
            {
                if (!GetConsoleCursorInfo(_consoleBuffer.BufferHandle, out var cursorInfo))
                {
                    throw GetLastError().GetException();
                }
                cursorInfo.bVisible = false;
                if (!SetConsoleCursorInfo(_consoleBuffer.BufferHandle, cursorInfo))
                {
                    throw GetLastError().GetException();
                }
            }
        }

        public void ShowCaret()
        {
            lock (_consoleBuffer.BufferHandle)
            {
                if (!GetConsoleCursorInfo(_consoleBuffer.BufferHandle, out var cursorInfo))
                {
                    throw GetLastError().GetException();
                }
                cursorInfo.bVisible = true;
                if (!SetConsoleCursorInfo(_consoleBuffer.BufferHandle, cursorInfo))
                {
                    throw GetLastError().GetException();
                }
            }
        }

        public void WriteText(string str)
        {
            throw new NotImplementedException("This doesn't support escape codes, you should use Print instead.");
        }
    }
}
