using System;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    /// IConsoleOutput implementation which purely uses Console API and not ANSI escape sequences.
    /// </summary>
    /// <remarks>
    /// This only supports ConsoleColor and not mouse or other advanced features.
    /// </remarks>
    public class DefaultNetConsoleOutput : IConsoleOutput
    {
        private ConsoleColor _originalForeground;
        private ConsoleColor _originalBackground;
        private bool _caretVisible;
        protected Task PauseTask { get; private set; }

        public event Action Resized;

        
        public DefaultNetConsoleOutput()
        {
        }

        public PixelBufferSize Size { get; private set; }

        public bool CaretVisible => _caretVisible;

        public bool SupportsComplexEmoji => false;

        public bool SupportsAltSolo => false;

        public virtual void PauseIO(Task task)
        {
            task.ContinueWith(_ => { PauseTask = null; }, TaskScheduler.Default);
            PauseTask = task;
        }

        public void SetTitle(string title)
            => Console.Title = title;

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
            => Console.SetCursorPosition(bufferPoint.X, bufferPoint.Y);

        public PixelBufferCoordinate GetCaretPosition()
        {
            var (left, top) = Console.GetCursorPosition();
            return new PixelBufferCoordinate((ushort)left, (ushort)top);
        }

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style, FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            var (consoleColor, egaColor) = EgaConsoleColorMode.ConvertToConsoleColorMode(foreground);
            Console.ForegroundColor = consoleColor;
            (consoleColor, egaColor) = EgaConsoleColorMode.ConvertToConsoleColorMode(background);
            Console.BackgroundColor = consoleColor;
            Console.SetCursorPosition(bufferPoint.X, bufferPoint.Y);
            Console.Write(str);
        }

        public void WriteText(string str)
        {
            PauseTask?.Wait();
            Console.Write(str);
        }

        public void SetCaretStyle(CaretStyle caretStyle)
        {
        }

        public void HideCaret()
        {
            Console.CursorVisible = false;
            _caretVisible = false;
        }

        public void ShowCaret()
        {
            Console.CursorVisible = true;
            _caretVisible = true;
        }

        public void PrepareConsole()
        {
            Console.OutputEncoding = Encoding.UTF8;

            _originalForeground = Console.ForegroundColor;
            _originalBackground = Console.BackgroundColor;
            CheckSize();
            Console.Clear();
        }

        public void RestoreConsole()
        {
            Console.ForegroundColor = _originalForeground;
            Console.BackgroundColor = _originalBackground;
        }

        public void ClearScreen()
        {
            Console.Clear();
        }

        public bool CheckSize()
        {
            if (Size.Width == Console.WindowWidth && Size.Height == Console.WindowHeight) return false;

            Size = new PixelBufferSize((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);
            Resized?.Invoke();
            return true;
        }
    }
}