using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Text;
using NeoSmart.Unicode;

namespace Consolonia.Core.Infrastructure
{
    public class InputLessDefaultNetConsole : IConsole
    {
        private bool _caretVisible;
        private PixelBufferCoordinate _headBufferPoint;

        protected InputLessDefaultNetConsole()
        {
            Console.CursorVisible = false;
            ActualizeSize();
        }

        protected bool Disposed { get; private set; }

        protected Task PauseTask { get; private set; }

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

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle style,
            FontWeight weight, TextDecorationCollection textDecorations, string str)
        {
            PauseTask?.Wait();
            SetCaretPosition(bufferPoint);

            var sb = new StringBuilder();
            if (HasTextDecoration(textDecorations, TextDecorationLocation.Underline))
                sb.Append(ConsoleUtils.Underline);

            if (HasTextDecoration(textDecorations, TextDecorationLocation.Strikethrough))
                sb.Append(ConsoleUtils.Strikethrough);

            if (style == FontStyle.Italic)
                sb.Append(ConsoleUtils.Italic);

            sb.Append(ConsoleUtils.Background(background));

            sb.Append(ConsoleUtils.Foreground(weight switch
            {
                FontWeight.Medium or FontWeight.SemiBold or FontWeight.Bold or FontWeight.ExtraBold or FontWeight.Black
                    or FontWeight.ExtraBlack
                    => foreground.Brighten(background),
                FontWeight.Thin or FontWeight.ExtraLight or FontWeight.Light
                    => foreground.Shade(background),
                _ => foreground
            }));

            sb.Append(str);
            sb.Append(ConsoleUtils.Reset);

            Console.Write(sb.ToString());

            if (_headBufferPoint.X < Size.Width - str.Length)
                _headBufferPoint =
                    new PixelBufferCoordinate((ushort)(_headBufferPoint.X + str.Length), _headBufferPoint.Y);
            else
                _headBufferPoint = (PixelBufferCoordinate)((ushort)0, (ushort)(_headBufferPoint.Y + 1));
        }

        public event Action Resized;
        public event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        public event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;
        public event Action<bool> FocusEvent;

        public virtual void PauseIO(Task task)
        {
            task.ContinueWith(_ => { PauseTask = null; }, TaskScheduler.Default);
            PauseTask = task;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void ClearOutput()
        {
            // this is hack, but somehow it does not work when just calling ActualizeSize with same size
            Size = new PixelBufferSize(1, 1);
            Resized?.Invoke();
        }

        private static bool HasTextDecoration(TextDecorationCollection textDecorations, TextDecorationLocation location)
        {
            return textDecorations?.Any(td => td.Location == location) ?? false;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        protected bool CheckActualizeTheSize()
        {
            if (Size.Width == Console.WindowWidth && Size.Height == Console.WindowHeight) return false;
            ActualizeSize();
            return true;
        }

        protected void ActualizeSize()
        {
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
                    Task pauseTask = PauseTask;
                    if (pauseTask != null)
                        await pauseTask.ConfigureAwait(false);

                    int timeout = (int)(CheckActualizeTheSize() ? 1 : slowInterval);
                    await Task.Delay(timeout).ConfigureAwait(false);
                }
            });
        }
    }
}