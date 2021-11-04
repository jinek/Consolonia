using System;
using System.Threading.Tasks;
using Avalonia;

namespace Consolonia.Core.Infrastructure
{
    internal class DefaultNetConsole : IConsole
    {
        public DefaultNetConsole()
        {
            Console.CursorVisible = false;
            InitializeCache();
            StartSizeCheckTimer();
        }

        private async void StartSizeCheckTimer()
        {
            ActualizeSize();
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            while (true)
            {
                if (width != Console.WindowWidth || height != Console.WindowHeight)
                {
                    ActualizeSize();

                    Resized?.Invoke();
                    InitializeCache();
                }

                await Task.Delay(1000);
            }

            void ActualizeSize()
            {
                Console.Clear();
                width = Console.WindowWidth;
                height = Console.WindowHeight;
            }
        }


        public void MoveCaretForControl(Point position, int size, object ownerControl)
        {
            //todo: must be int top,left instead of position
            //todo: check if ownerControl is current caret showing, otherwise exception
            // Console.CursorSize = size; todo: not supported on linux
            SetCaretPosition(((ushort)position.X, (ushort)position.Y));
        }

        private object _carrentControlOfCaret;

        public void AddCaretFor(object control)
        {
            /*todo: if (_carrentControlOfCaret != null)
                throw new NotSupportedException();*/
            _carrentControlOfCaret = control;
            Console.CursorVisible = true;
        }

        public void RemoveCaretFor(object control)
        {
            /*todo: if (_carrentControlOfCaret == null)
                throw new NotSupportedException(); */
            _carrentControlOfCaret = null;
            Console.CursorVisible = false;
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

        private (ushort x, ushort y) _headPosition;
        private ConsoleColor _headBackground;
        private ConsoleColor _headForeground;
        private (ConsoleColor background, ConsoleColor foreground, char character)?[,] _cache;

        private void InitializeCache()
        {
            _cache = new (ConsoleColor background, ConsoleColor foreground, char character)?[Size.width, Size.height];
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
    }
}