using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NullLib.ConsoleEx;

namespace Consolonia.Core.Infrastructure
{
    public class InputLessDefaultNetConsole : IConsole
    {
        private bool _caretVisible;
        private ConsoleColor _headBackground;
        private PixelBufferCoordinate _headBufferPoint;
        private ConsoleColor _headForeground;

        protected InputLessDefaultNetConsole()
        {
            Console.CursorVisible = false;
            ActualizeTheSize();
        }

        protected bool Disposed { get; private set; }

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

        public PixelBufferSize Size { get; private set; }

        public void SetTitle(string title)
        {
            Console.Title = title;
        }

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

            if (!str.IsNormalized(NormalizationForm.FormKC))
                str = str.Normalize(NormalizationForm.FormKC);

            if (str.Any(
                c => ConsoleText.IsWideChar(c) &&
                     char.IsLetterOrDigit(c) /*todo: https://github.com/SlimeNull/NullLib.ConsoleEx/issues/2*/))
            {
                StringBuilder stringBuilder = new();
                foreach (char c in str)
                    stringBuilder.Append(ConsoleText.IsWideChar(c) && char.IsLetterOrDigit(c)
                        ? '?' //todo: support wide characters
                        : c);

                str = stringBuilder.ToString();
            }

            Console.Write(str);

            if (_headBufferPoint.X < Size.Width - str.Length)
                _headBufferPoint =
                    new PixelBufferCoordinate((ushort)(_headBufferPoint.X + str.Length), _headBufferPoint.Y);
            else _headBufferPoint = (PixelBufferCoordinate)((ushort)0, (ushort)(_headBufferPoint.Y + 1));
        }

        public event Action Resized;
        public event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        public event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;
        public event Action<bool> FocusEvent;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected bool CheckActualizeTheSize()
        {
            if (Size.Width == Console.WindowWidth && Size.Height == Console.WindowHeight) return false;
            ActualizeTheSize();
            return true;
        }

        protected void ActualizeTheSize()
        {
            Console.Clear();
            Size = new PixelBufferSize((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);
            Resized?.Invoke();
        }

        protected void RaiseMouseEvent(RawPointerEventType eventType, Point point, Vector? wheelDelta,
            RawInputModifiers modifiers)
        {
            MouseEvent?.Invoke(eventType, point, wheelDelta, modifiers);
        }

        protected void RaiseKeyPress(Key key, char character, RawInputModifiers modifiers, bool down, ulong timeStamp)
        {
            KeyEvent?.Invoke(key, character, modifiers, down, timeStamp);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Disposed = true;
        }

        protected void RaiseFocusEvent(bool focused)
        {
            FocusEvent?.Invoke(focused);
        }

        protected void StartSizeCheckTimerAsync(uint slowInterval = 1500)
        {
            Task.Run(async () =>
            {
                while (!Disposed)
                {
                    int timeout = (int)(CheckActualizeTheSize() ? 1 : slowInterval);
                    await Task.Delay(timeout).ConfigureAwait(false);
                }
            });
        }
    }
}