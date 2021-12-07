using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Core.Drawing.PixelBuffer;

namespace Consolonia.Core.Infrastructure
{
    internal class DefaultNetConsole : IConsole
    {
        private (ConsoleColor background, ConsoleColor foreground, char character)?[,] _cache;
        private bool _caretVisible;
        private ConsoleColor _headBackground;
        private ConsoleColor _headForeground;
        private PixelBufferCoordinate _headBufferPoint;


        public DefaultNetConsole()
        {
            Console.CursorVisible = false;
            InitializeCache();
            StartSizeCheckTimer();

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
            Console.SetCursorPosition(bufferPoint.X, bufferPoint.Y);
        }

        public PixelBufferCoordinate GetCaretPosition()
        {
            return _headBufferPoint;
        }

        public void Print(PixelBufferCoordinate bufferPoint, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            char character)
        {
            SetCaretPosition(bufferPoint);
            (ushort x, ushort y) = bufferPoint;

            if (_headBackground != backgroundColor)
                _headBackground = Console.BackgroundColor = backgroundColor;
            if (_headForeground != foregroundColor)
                _headForeground = Console.ForegroundColor = foregroundColor;

            if (_cache[x, y] == (backgroundColor, foregroundColor, character))
                return;

            _cache[x, y] = (backgroundColor, foregroundColor, character);
            Console.Write(character);
            if (_headBufferPoint.X < Size.Width - 1)
                _headBufferPoint = _headBufferPoint.WithXpp();
            else _headBufferPoint = (PixelBufferCoordinate)((ushort)0, (ushort)(_headBufferPoint.Y + 1));
        }

        public event Action? Resized;

        public PixelBufferSize Size =>
            new((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);

        private void StartInputReading()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);

                    if (!KeyMapping.TryGetValue(consoleKeyInfo.Key, out Key key))
                    {
                        if (!Enum.TryParse(consoleKeyInfo.Key.ToString(), out key))
                            throw new NotImplementedException();
                    }

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

        private static readonly Dictionary<ConsoleKey, Key> KeyMapping = new()
        {
            {ConsoleKey.Spacebar, Key.Space},
            {ConsoleKey.RightArrow, Key.Right},
            {ConsoleKey.LeftArrow, Key.Left},
            {ConsoleKey.UpArrow, Key.Up},
            {ConsoleKey.DownArrow, Key.Down},
            {ConsoleKey.Backspace, Key.Back},
        };

        private async void StartSizeCheckTimer()
        {
            ActualizeSize();
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            while (true)
            {
                int timeout;
                if (width != Console.WindowWidth || height != Console.WindowHeight)
                {
                    ActualizeSize();

                    Resized?.Invoke();
                    InitializeCache();
                    timeout = 1; //todo: magic numbers. probably need to rely on fps instead
                }
                else
                {
                    timeout = 1000;
                }

                await Task.Delay(timeout);
            }

            void ActualizeSize()
            {
                Console.Clear();
                width = Console.WindowWidth;
                height = Console.WindowHeight;
            }
        }

        private void InitializeCache()
        {
            _cache = new (ConsoleColor background, ConsoleColor foreground, char character)?[Size.Width, Size.Height];
        }
    }
}