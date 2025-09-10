using System;
using System.Diagnostics;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Text;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     Console implementation which uses ANSI escape sequences for output
    /// </summary>
    public class AnsiConsoleOutput : IConsoleOutput
    {
        private const string TestEmoji = "👨‍👩‍👧‍👦";
        private PixelBufferCoordinate _headBufferPoint;

        private bool? _supportEmoji;

        public bool SupportsComplexEmoji => _supportEmoji ?? false;

        public PixelBufferSize Size { get; set; }

        public void SetTitle(string title)
        {
            WriteText(Esc.SetWindowTitle(title));
        }

        public void SetCaretPosition(PixelBufferCoordinate bufferPoint)
        {
            if (bufferPoint.Equals(GetCaretPosition())) return;

            try
            {
                WriteText(Esc.SetCursorPosition(bufferPoint.X, bufferPoint.Y));
                _headBufferPoint = bufferPoint;
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

        private static Lazy<IConsoleColorMode> _consoleColorMode =
            new(() => AvaloniaLocator.Current.GetRequiredService<IConsoleColorMode>());

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style,
            FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            //todo: performance of retrieval of the service, at least can be retrieved once
            var consoleColorMode = _consoleColorMode;

            SetCaretPosition(bufferPoint);

            StringBuilder sb = new StringBuilder();
            if (textDecoration == TextDecorationLocation.Underline)
                sb.Append(Esc.Underline);

            if (textDecoration == TextDecorationLocation.Strikethrough)
                sb.Append(Esc.Strikethrough);

            if (style == FontStyle.Italic)
                sb.Append(Esc.Italic);

            (object mappedBackground, object mappedForeground) =
                consoleColorMode.Value.MapColors(background, foreground, weight);
            sb.Append(Esc.Foreground(mappedForeground));
            sb.Append(Esc.Background(mappedBackground));
            sb.Append(str);
            sb.Append(Esc.Reset);
            
            WriteText(sb.ToString());

            ushort textWidth = str.MeasureText();
            if (_headBufferPoint.X < Size.Width - textWidth)
                _headBufferPoint =
                    new PixelBufferCoordinate((ushort)(_headBufferPoint.X + textWidth), _headBufferPoint.Y);
            else
                _headBufferPoint = (PixelBufferCoordinate)((ushort)0, (ushort)(_headBufferPoint.Y + 1));
        }

        /// <summary>
        ///     Write raw text to the console
        /// </summary>
        /// <remarks>This does not move the caret position, so should only be used for escape commands</remarks>
        /// <param name="str"></param>
        public void WriteText(string str)
        {
            Console.Write(str);
        }

        public void PrepareConsole()
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.OutputEncoding = Encoding.UTF8;

            // enable alternate screen so original console screen is not affected by the app
            WriteText(Esc.EnableAlternateBuffer);

            Size = new PixelBufferSize((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);

            // Detect complex emoji support by writing a complex emoji and checking cursor position.
            // If the cursor moves 2 positions, it indicates proper rendering of composite surrogate pairs.
            (int left, _) = Console.GetCursorPosition();
            WriteText(TestEmoji);
            (int left2, _) = Console.GetCursorPosition();
            _supportEmoji = left2 - left == 2;

            ClearScreen();
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        public void RestoreConsole()
        {
            WriteText(Esc.DisableAlternateBuffer);
            WriteText(Esc.Reset);
            WriteText(Esc.ShowCursor);
        }

        public void SetCaretStyle(CaretStyle caretStyle)
        {
            switch (caretStyle)
            {
                case CaretStyle.BlinkingBlock:
                    WriteText(Esc.BlinkingBlockCursor);
                    break;
                case CaretStyle.SteadyBlock:
                    WriteText(Esc.SteadyBlockCursor);
                    break;
                case CaretStyle.BlinkingUnderline:
                    WriteText(Esc.BlinkingUnderlineCursor);
                    break;
                case CaretStyle.SteadyUnderline:
                    WriteText(Esc.SteadyUnderlineCursor);
                    break;
                case CaretStyle.BlinkingBar:
                    WriteText(Esc.BlinkingBarCursor);
                    break;
                case CaretStyle.SteadyBar:
                    WriteText(Esc.SteadyBarCursor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(caretStyle), caretStyle, null);
            }
        }

        public void HideCaret()
        {
            WriteText(Esc.HideCursor);
        }

        public void ShowCaret()
        {
            WriteText(Esc.ShowCursor);
        }

        public void ClearScreen()
        {
            WriteText(Esc.ClearScreen);
            _headBufferPoint = new PixelBufferCoordinate(0, 0);
            WriteText(Esc.SetCursorPosition(0, 0));
        }
    }
}