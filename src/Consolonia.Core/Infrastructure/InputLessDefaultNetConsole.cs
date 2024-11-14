using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;
using Consolonia.Core.Text;

namespace Consolonia.Core.Infrastructure
{
    public class InputLessDefaultNetConsole : IConsole
    {
        private const string TestEmoji = "👨‍👩‍👧‍👦";
        private bool _caretVisible;
        private PixelBufferCoordinate _headBufferPoint;

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        protected InputLessDefaultNetConsole()
        {
            Console.OutputEncoding = Encoding.UTF8;
            // enable alternate screen so original console screen is not affected by the app
            Console.Write(ConsoleUtils.EnableAlternateBuffer);
            Console.Write(ConsoleUtils.HideCursor);
            Console.Write(ConsoleUtils.ClearScreen);

            ActualizeSize();
        }
#pragma warning restore CA1303 // Do not pass literals as localized parameters

        protected bool Disposed { get; private set; }

        protected Task PauseTask { get; private set; }

        public bool CaretVisible
        {
            get => _caretVisible;
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            set
            {
                if (_caretVisible == value) return;
                Console.Write(value ? ConsoleUtils.ShowCursor : ConsoleUtils.HideCursor);
                _caretVisible = value;
            }
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        public PixelBufferSize Size { get; private set; }

        private bool? _supportEmoji;
        public bool SupportsComplexEmoji
        {
            get
            {
                if (_supportEmoji == null)
                {
                    var lifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                    if (lifetime.Args.Contains("--buffer"))
                        _supportEmoji = true;
                    else
                    {
                        // Detect complex emoji support by writing a complex emoji and checking cursor position.
                        // If the cursor moves 2 positions, it indicates proper rendering of composite surrogate pairs.
                        var (left, top) = Console.GetCursorPosition();
                        Console.Write(TestEmoji);

                        // TODO, escape sequence
                        var (left2, _) = Console.GetCursorPosition();
                        _supportEmoji = left2 - left == 2;
                        Console.SetCursorPosition(left, top);
                    }
                }
                return _supportEmoji ?? true;
            }
            private set
            {
                _supportEmoji = value;
            }
        }

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
            FontWeight weight, TextDecorationLocation? textDecoration, string str)
        {
            PauseTask?.Wait();
            SetCaretPosition(bufferPoint);

            var sb = new StringBuilder();
            if (textDecoration == TextDecorationLocation.Underline)
                sb.Append(ConsoleUtils.Underline);

            if (textDecoration == TextDecorationLocation.Strikethrough)
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

            ushort textWidth = str.MeasureText();
            if (_headBufferPoint.X < Size.Width - textWidth)
                _headBufferPoint =
                    new PixelBufferCoordinate((ushort)(_headBufferPoint.X + textWidth), _headBufferPoint.Y);
            else
                _headBufferPoint = (PixelBufferCoordinate)((ushort)0, (ushort)(_headBufferPoint.Y + 1));
        }

        public void WriteCommand(string str)
        {
            PauseTask?.Wait();
            Console.Write(str);
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

#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1303 // Do not pass literals as localized parameters
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            Console.Write(ConsoleUtils.DisableAlternateBuffer);
            Console.Write(ConsoleUtils.ShowCursor);
        }
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CA1303 // Do not pass literals as localized parameters

        public void ClearOutput()
        {
            // this is hack, but somehow it does not work when just calling ActualizeSize with same size
            Size = new PixelBufferSize(1, 1);
            Resized?.Invoke();
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