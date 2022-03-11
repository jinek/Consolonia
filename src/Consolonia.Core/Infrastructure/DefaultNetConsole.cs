using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    internal class DefaultNetConsole : IConsole
    {
        private static readonly Dictionary<ConsoleKey, Key> KeyMapping = new()
        {
            { ConsoleKey.LeftWindows, Key.LWin },
            { ConsoleKey.RightWindows, Key.RWin },
            { ConsoleKey.Spacebar, Key.Space },
            { ConsoleKey.RightArrow, Key.Right },
            { ConsoleKey.LeftArrow, Key.Left },
            { ConsoleKey.UpArrow, Key.Up },
            { ConsoleKey.DownArrow, Key.Down },
            { ConsoleKey.Backspace, Key.Back }
        };

        private bool _caretVisible;

        private bool _disposed;
        private ConsoleColor _headBackground;
        private PixelBufferCoordinate _headBufferPoint;
        private ConsoleColor _headForeground;


        public DefaultNetConsole()
        {
            Console.CursorVisible = false;
            StartSizeCheckTimerAsync();
            StartInputReading();
        }

        public bool CaretVisible
        {
            get => _caretVisible;
            set
            {
                if (_caretVisible == value) return;
                Console.CursorVisible = value;
                _caretVisible = value;
            }
        }

        public event Action<Key, char, RawInputModifiers> KeyPress;

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            if (bufferPoint.Equals(GetCaretPosition())) return;
            _headBufferPoint = bufferPoint;

            try
            {
                Console.SetCursorPosition(bufferPoint.X, bufferPoint.Y);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                if (bufferPoint.X < 0 || bufferPoint.Y < 0)
                    throw;
                throw new InvalidDrawingContextException("Window has been resized probably",
                    argumentOutOfRangeException);
            }
        }

        public PixelBufferCoordinate GetCaretPosition()
        {
            return _headBufferPoint;
        }

        public void Print(PixelBufferCoordinate bufferPoint, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            string str)
        {
            SetCaretPosition(bufferPoint);

            if (_headBackground != backgroundColor)
                _headBackground = Console.BackgroundColor = backgroundColor;
            if (_headForeground != foregroundColor)
                _headForeground = Console.ForegroundColor = foregroundColor;

            Console.Write(str);

            if (_headBufferPoint.X < Size.Width - str.Length)
                _headBufferPoint =
                    new PixelBufferCoordinate((ushort)(_headBufferPoint.X + str.Length), _headBufferPoint.Y);
            else _headBufferPoint = (PixelBufferCoordinate)((ushort)0, (ushort)(_headBufferPoint.Y + 1));
        }

        public event Action Resized;

        public PixelBufferSize Size { get; private set; }

        public void Dispose()
        {
            _disposed = true;
        }


        private void StartInputReading()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (!_disposed)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

                    if (!KeyMapping.TryGetValue(consoleKeyInfo.Key, out Key key))
                        if (!Enum.TryParse(consoleKeyInfo.Key.ToString(), out key))
                            throw new NotImplementedException();

                    var rawInputModifiers = RawInputModifiers.None;

                    if (consoleKeyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                        rawInputModifiers |= RawInputModifiers.Control;
                    if (consoleKeyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        rawInputModifiers |= RawInputModifiers.Shift;
                    if (consoleKeyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt))
                        rawInputModifiers |= RawInputModifiers.Alt;

                    KeyPress?.Invoke(key, consoleKeyInfo.KeyChar, rawInputModifiers);
                }
            });
        }

        private void StartSizeCheckTimerAsync()
        {
            ActualizeSize();

            Task.Run(async () =>
            {
                while (!_disposed)
                {
                    int timeout;
                    if (Size.Width != Console.WindowWidth || Size.Height != Console.WindowHeight)
                    {
                        ActualizeSize();

                        await Dispatcher.UIThread.InvokeAsync(() => { Resized?.Invoke(); })
                            .ConfigureAwait(false /*we are fine to check size on any thread*/);

                        timeout = 1; //todo: magic numbers. probably need to rely on fps instead
                    }
                    else
                    {
                        timeout = 1000;
                    }

                    await Task.Delay(timeout).ConfigureAwait(false);
                }
            });

            void ActualizeSize()
            {
                Console.Clear();
                Size = new PixelBufferSize((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);
            }
        }
    }
}