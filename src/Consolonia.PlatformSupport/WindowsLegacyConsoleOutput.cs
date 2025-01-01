#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Vanara.PInvoke;
using static Vanara.PInvoke.Kernel32;
using static Vanara.PInvoke.User32;

namespace Consolonia.PlatformSupport
{

    internal class WindowsLegacyConsoleOutput : IConsoleOutput
    {
        private WindowsConsoleBuffer _originalBuffer;
        private WindowsConsoleBuffer _consoleBuffer;

        private CancellationTokenSource _taskCompletionSource = new CancellationTokenSource();

        public event Action Resized;


        public PixelBufferSize Size
        {
            get
            {
                var info = new CONSOLE_SCREEN_BUFFER_INFOEX();
                info.cbSize = (uint)Marshal.SizeOf(info);
                if (!GetConsoleScreenBufferInfoEx(_consoleBuffer.BufferHandle, ref info))
                {
                    throw GetLastError().GetException();
                }

                return new PixelBufferSize((ushort)info.dwSize.X, (ushort)info.dwSize.Y);
            }
        }

        public bool CaretVisible
        {
            get
            {
                var info = new CONSOLE_CURSOR_INFO();
                if (!GetConsoleCursorInfo(_consoleBuffer.BufferHandle, out info))
                {
                    throw GetLastError().GetException();
                }
                return info.bVisible;
            }
        }

        public bool SupportsComplexEmoji => false;

        public bool SupportsAltSolo => false;

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

            _ = MonitorWindowTask(_taskCompletionSource.Token);
        }

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style, FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            var consoleColorMode = AvaloniaLocator.Current.GetRequiredService<IConsoleColorMode>();

            var (b, f) = consoleColorMode.MapColors(background, foreground, weight);

            if (!(b is ConsoleColor backgroundConsoleColor))
                throw new InvalidCastException("Background color must be ConsoleColor");

            if (!(f is ConsoleColor foregroundConsoleColor))
                throw new InvalidCastException("Foreground color must be ConsoleColor");

            Console.BackgroundColor = backgroundConsoleColor;
            Console.ForegroundColor = foregroundConsoleColor;

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

        private RECT _previousSize;
        public virtual bool CheckSize()
        {
            var consoleWindow = GetConsoleWindow();

            lock (this)
            {
                if (!GetWindowRect(consoleWindow, out RECT currentRect))
                {
                    int error = Marshal.GetLastWin32Error();
                    System.Diagnostics.Debug.WriteLine($"Failed to get console window rect. Error: {error}");
                }

                if (currentRect.Width != _previousSize.Width || currentRect.Height != _previousSize.Height)
                {
                    GetCurrentConsoleFont(_consoleBuffer.BufferHandle, false, out var fontInfo);
                    var newSize = new COORD((short)(currentRect.Width / fontInfo.dwFontSize.X) - 10, (short)(currentRect.Height / fontInfo.dwFontSize.Y) - 10);
                    System.Diagnostics.Debug.WriteLine($"{Console.WindowWidth}x{Console.WindowHeight} {currentRect.Width}x{currentRect.Height} NewSize: {newSize.X}x{newSize.Y}");
                    //if (!SetConsoleScreenBufferSize(_consoleBuffer.BufferHandle, newSize))
                    //{
                    //    System.Diagnostics.Debug.WriteLine($"Failed to set size to {newSize.X}x{newSize.Y}");
                    //}
                    _previousSize = currentRect;

                    Resized?.Invoke();
                    return true;
                }
            }
            return false;
        }
        //             var size = this.Size;
        //System.Diagnostics.Debug.WriteLine($"{size.Width}x{size.Height} => Console: {Console.WindowWidth}x{Console.WindowHeight}");
        //if (size.Width == Console.WindowWidth && size.Height == Console.WindowHeight) return false;

        //// Set the window size to match the buffer size
        //SMALL_RECT windowSize = new SMALL_RECT
        //{
        //    Left = 0,
        //    Top = 0,
        //    Right = (short)(size.Width - 1),
        //    Bottom = (short)(size.Height - 1)
        //};

        //Console.WindowWidth = size.Width;
        //Console.WindowHeight = size.Height;
        //Resized?.Invoke();

        public void WriteText(string str)
        {
            throw new NotImplementedException("This doesn't support escape codes, you should use Print instead.");
        }

        private async Task MonitorWindowTask(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                CheckSize();

                // Polling interval
                await Task.Delay(500);
            }
        }
    }
}
