using System;
using System.Text;
using Avalonia.Media;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     IConsoleOutput implementation which purely uses Console API and not ANSI escape sequences.
    /// </summary>
    /// <remarks>
    ///     This only supports ConsoleColor and not mouse or other advanced features.
    /// </remarks>
    public class DefaultNetConsoleOutput : IConsoleOutput
    {
        private ConsoleColor _originalBackground;
        private ConsoleColor _originalForeground;


        public virtual PixelBufferSize Size { get; set; }

        public virtual bool SupportsComplexEmoji => false;

        public virtual void SetTitle(string title)
        {
            Console.Title = title;
        }

        public virtual void SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            try
            {
                Console.SetCursorPosition(bufferPoint.X, bufferPoint.Y);
            }
            catch (ArgumentOutOfRangeException)
            {
                // ignore
                // this happens while resizing.
            }
        }

        public virtual PixelBufferCoordinate GetCaretPosition()
        {
            (int left, int top) = Console.GetCursorPosition();
            return new PixelBufferCoordinate((ushort)left, (ushort)top);
        }

        public virtual void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground,
            FontStyle? style,
            FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;

            (ConsoleColor consoleColor, _) = EgaConsoleColorMode.ConvertToConsoleColorMode(foreground);
            Console.ForegroundColor = consoleColor;
            (consoleColor, _) = EgaConsoleColorMode.ConvertToConsoleColorMode(background);
            Console.BackgroundColor = consoleColor;

            SetCaretPosition(bufferPoint);
            Console.Write(str);

            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
        }

        public virtual void WriteText(string str)
        {
            Console.Write(str);
        }

        public virtual void SetCaretStyle(CaretStyle caretStyle)
        {
            try
            {
#pragma warning disable CA1416 // Validate platform compatibility
                Console.CursorSize = caretStyle switch
                {
                    CaretStyle.SteadyBlock => 100,
                    CaretStyle.BlinkingBlock => 100,
                    CaretStyle.SteadyUnderline => 1,
                    CaretStyle.BlinkingUnderline => 1,
                    CaretStyle.SteadyBar => 50,
                    CaretStyle.BlinkingBar => 50,
                    _ => throw new ArgumentOutOfRangeException(nameof(caretStyle))
                };
#pragma warning restore CA1416 // Validate platform compatibility
            }
            catch (PlatformNotSupportedException)
            {
                //todo: should be Consolonia exception? Should we avoid custom CursorSize on higher level in Theme for instance?
            }
        }

        public virtual void HideCaret()
        {
            Console.CursorVisible = false;
        }

        public virtual void ShowCaret()
        {
            Console.CursorVisible = true;
        }

        public virtual void PrepareConsole()
        {
            Console.OutputEncoding = Encoding.UTF8;

            _originalForeground = Console.ForegroundColor;
            _originalBackground = Console.BackgroundColor;
            Size = new PixelBufferSize((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);
            Console.Clear();
        }

        public virtual void RestoreConsole()
        {
            Console.ForegroundColor = _originalForeground;
            Console.BackgroundColor = _originalBackground;
        }

        public virtual void ClearScreen()
        {
            Console.Clear();
        }
    }
}