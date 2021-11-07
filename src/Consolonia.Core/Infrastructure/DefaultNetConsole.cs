using System;
using System.Threading.Tasks;
using Avalonia;

namespace Consolonia.Core.Infrastructure
{
    internal class DefaultNetConsole : IConsole
    {
        private (ConsoleColor background, ConsoleColor foreground, char character)?[,] _cache;
        private object _currentControlOfCaret;
        private ConsoleColor _headBackground;
        private ConsoleColor _headForeground;
        private (ushort x, ushort y) _headPosition;
        private bool _caretVisible;

        public DefaultNetConsole()
        {
            Console.CursorVisible = false;
            InitializeCache();
            StartSizeCheckTimer();
        }


        public void MoveCaretForControl(Point? position, int size, object ownerControl)
        {
            if (_currentControlOfCaret != ownerControl)
                throw new InvalidOperationException();

            //todo: must be int top,left instead of position
            // Console.CursorSize = size; todo: not supported on linux
            if (position == null)
                HideCaretInternal();
            else
            {
                ShowCaretInternal();
                (double x, double y) = (Point)position;
                SetCaretPosition(((ushort)x, (ushort)y));
            }
        }

        public void AddCaretFor(object control)
        {
            if (_currentControlOfCaret != null)
                throw new NotSupportedException();

            _currentControlOfCaret = control;

            ShowCaretInternal();
        }

        private void ShowCaretInternal()
        {
            if (_caretVisible)
                return;
            _caretVisible = Console.CursorVisible = true;
        }

        public void RemoveCaretFor(object control)
        {
            if (_currentControlOfCaret == null)
                throw new NotSupportedException();

            _currentControlOfCaret = null;
            HideCaretInternal();
        }

        private void HideCaretInternal()
        {
            if (!_caretVisible)
                return;
            _caretVisible = Console.CursorVisible = false;
        }

        public IDisposable StoreCaret()
        {
            return new CaretStorage(this);
        }

        public void SetCaretPosition((ushort x, ushort y) position)
        {
            (ushort left, ushort top) = GetCaretPosition();
            (ushort x, ushort y) = position;
            if (left == x && top == y) return;
            _headPosition = (x, y);
            Console.SetCursorPosition(x, y);
        }

        public (ushort x, ushort y) GetCaretPosition()
        {
            return _headPosition;
        }

        public void Print(ushort x, ushort y, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            char character)
        {
            SetCaretPosition((x, y));

            if (_headBackground != backgroundColor)
                _headBackground = Console.BackgroundColor = backgroundColor;
            if (_headForeground != foregroundColor)
                _headForeground = Console.ForegroundColor = foregroundColor;

            if (_cache[x, y] == (backgroundColor, foregroundColor, character))
                return;

            _cache[x, y] = (backgroundColor, foregroundColor, character);
            Console.Write(character);
            if (_headPosition.x < Size.width - 1)
                _headPosition.x++;
            else _headPosition = ((ushort x, ushort y))(0, _headPosition.y + 1);
        }

        public event Action? Resized;

        public (ushort width, ushort height) Size =>
            ((ushort width, ushort height))(Console.WindowWidth, Console.WindowHeight);

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
                    timeout = 1;//todo: magic numbers. probably need to rely on fps instead
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
            _cache = new (ConsoleColor background, ConsoleColor foreground, char character)?[Size.width, Size.height];
        }
    }
}