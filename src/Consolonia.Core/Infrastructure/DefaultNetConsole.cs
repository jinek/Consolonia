using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure
{
    internal class DefaultNetConsole : IConsole
    {
        private (ConsoleColor background, ConsoleColor foreground, char character)?[,] _cache;
        private bool _caretVisible;
        private object _currentControlOfCaret;
        private ConsoleColor _headBackground;
        private ConsoleColor _headForeground;
        private ConsolePosition _headPosition;


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


        public void MoveCaretForControl(ConsolePosition? position, int size, object ownerControl)
        {
            /*if (_currentControlOfCaret != ownerControl)
                throw new InvalidOperationException();*/

            // Console.CursorSize = size; not supported on linux
            if (position == null)
            {
                CaretVisible = false;
            }
            else
            {
                CaretVisible = true;
                SetCaretPosition((ConsolePosition)position);
            }
        }

        public void AddCaretFor(object control)
        {
            // if (_currentControlOfCaret != null)
            // throw new NotSupportedException();

            _currentControlOfCaret = control;

            CaretVisible = true;
        }


        public void RemoveCaretFor(object control)
        {
            /*if (_currentControlOfCaret == null)
                throw new NotSupportedException();*/

            _currentControlOfCaret = null;
            CaretVisible = false;
        }

        public IDisposable StoreCaret()
        {
            return new CaretStorage(this);
        }

        public void SetCaretPosition(ConsolePosition position)
        {
            if (position.Equals(GetCaretPosition())) return;
            _headPosition = position;
            Console.SetCursorPosition(position.X, position.Y);
        }

        public ConsolePosition GetCaretPosition()
        {
            return _headPosition;
        }

        public void Print(ConsolePosition position, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            char character)
        {
            SetCaretPosition(position);
            (ushort x, ushort y) = position;

            if (_headBackground != backgroundColor)
                _headBackground = Console.BackgroundColor = backgroundColor;
            if (_headForeground != foregroundColor)
                _headForeground = Console.ForegroundColor = foregroundColor;

            if (_cache[x, y] == (backgroundColor, foregroundColor, character))
                return;

            _cache[x, y] = (backgroundColor, foregroundColor, character);
            Console.Write(character);
            if (_headPosition.X < Size.Width - 1)
                _headPosition = _headPosition.WithXpp();
            else _headPosition = (ConsolePosition)((ushort)0, (ushort)(_headPosition.Y + 1));
        }

        public event Action? Resized;

        public ConsoleSize Size =>
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