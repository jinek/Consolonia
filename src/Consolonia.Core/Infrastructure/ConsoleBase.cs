using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// Base IConsole implementation
    /// </summary>
    /// <remarks>
    /// This implements disposable and eventing for IConsoleInput and
    /// wraps around internal IConsoleOutput
    /// </remarks>
    public abstract class ConsoleBase : IConsole, IDisposable
    {
        private IConsoleOutput _consoleOutput;

        protected ConsoleBase(IConsoleOutput consoleOutput)
        {
            if (consoleOutput is ConsoleBase)
                throw new ArgumentException("ConsoleBase cannot be used as a console output", nameof(consoleOutput));

            _consoleOutput = consoleOutput;
            _consoleOutput.Resized += () => Resized?.Invoke();
        }

        protected bool Disposed { get; private set; }

        protected Task PauseTask { get; private set; }


        /// <summary>
        /// Pause the IO
        /// </summary>
        /// <param name="task"></param>
        public virtual void PauseIO(Task task)
        {
            task.ContinueWith(_ => { PauseTask = null; }, TaskScheduler.Default);
            PauseTask = task;
        }

        protected void StartSizeCheckTimerAsync(uint slowInterval = 1500)
        {
            Task.Run(async () =>
            {
                while (!Disposed)
                {
                    Task pauseTask = PauseTask;
                    if (pauseTask != null)
                        await pauseTask;

                    int timeout = (int)(_consoleOutput.CheckSize() ? 1 : slowInterval);
                    await Task.Delay(timeout);
                }
            });
        }

        #region IConsoleInput
        public abstract bool SupportsMouse { get; }

        public abstract bool SupportsMouseMove { get; }

        public event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        public event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;
        public event Action<bool> FocusEvent;

        protected void RaiseMouseEvent(RawPointerEventType eventType, Point point, Vector? wheelDelta,
            RawInputModifiers modifiers)
        {
            MouseEvent?.Invoke(eventType, point, wheelDelta, modifiers);
        }

        protected void RaiseKeyPress(Key key, char character, RawInputModifiers modifiers, bool down, ulong timeStamp)
        {
            KeyEvent?.Invoke(key, character, modifiers, down, timeStamp);
        }

        protected void RaiseFocusEvent(bool focused)
        {
            FocusEvent?.Invoke(focused);
        }
        #endregion

        #region IConsoleOutput
        public PixelBufferSize Size => _consoleOutput.Size;

        public bool CaretVisible => _consoleOutput.CaretVisible;

        public bool SupportsComplexEmoji => _consoleOutput.SupportsComplexEmoji;

        public bool SupportsAltSolo => _consoleOutput.SupportsAltSolo;

        public event Action Resized;

        public void ClearScreen()
            => _consoleOutput.ClearScreen();

        public PixelBufferCoordinate GetCaretPosition()
            => _consoleOutput.GetCaretPosition();

        public void HideCaret()
            => _consoleOutput.HideCaret();

        public void PrepareConsole()
            => _consoleOutput.PrepareConsole();

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style, FontWeight? weight, TextDecorationLocation? textDecoration, string str)
            => _consoleOutput.Print(bufferPoint, background, foreground, style, weight, textDecoration, str);

        public void RestoreConsole()
            => _consoleOutput.RestoreConsole();

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
            => _consoleOutput.SetCaretPosition(bufferPoint);

        public void SetCaretStyle(CaretStyle caretStyle)
            => _consoleOutput.SetCaretStyle(caretStyle);

        public void SetTitle(string title)
            => _consoleOutput.SetTitle(title);
        public void ShowCaret()
            => _consoleOutput.ShowCaret();

        public void WriteText(string str)
        {
            PauseTask?.Wait();
            _consoleOutput.WriteText(str);
        }
        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                RestoreConsole();

                Disposed = true;
            }
        }

        public void Dispose()
        {
#pragma warning disable CA1063 // Implement IDisposable Correctly
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Dispose(true);
            GC.SuppressFinalize(this);
#pragma warning restore CA1063 // Implement IDisposable Correctly
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        public bool CheckSize()
            => _consoleOutput.CheckSize();

        #endregion
    }
}