using System;
using System.Diagnostics;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;
using Consolonia.Core.Text;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     Console implementation which uses ANSI escape sequences for output
    /// </summary>
    /// <remarks>
    /// This console buffers all output and only writes to the console on Flush.
    /// </remarks>
    public class AnsiConsoleOutput : IConsoleOutput
    {
        private const string TestEmoji = "👨‍👩‍👧‍👦";

        private static readonly Lazy<IConsoleColorMode> ConsoleColorMode =
            new(() => AvaloniaLocator.Current.GetRequiredService<IConsoleColorMode>());

        private PixelBufferCoordinate _headBufferPoint;
        private StringBuilder _outputBuffer = new StringBuilder();
        private Color _lastForeground = Colors.Transparent;
        private Color _lastBackground = Colors.Transparent;
        private FontStyle? _lastStyle;
        private FontWeight? _lastWeight;
        private TextDecorationLocation? _lastTextDecoration;

        private bool? _supportsComplexEmoji;
        private bool? _supportsEmojiVariation;

        public bool SupportsComplexEmoji => _supportsComplexEmoji ?? false;

        public bool SupportsEmojiVariation => _supportsEmojiVariation ?? false;

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
                Debug.WriteLine($"Setting caret position to {bufferPoint.X},{bufferPoint.Y}");
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

        public void Print(PixelBufferCoordinate bufferPoint, Color background, Color foreground, FontStyle? style,
            FontWeight? weight, TextDecorationLocation? textDecoration, string str)
        {
            //todo: performance of retrieval of the service, at least can be retrieved once
            Lazy<IConsoleColorMode> consoleColorMode = ConsoleColorMode;

            if (bufferPoint != _headBufferPoint)
            {
                SetCaretPosition(bufferPoint);
            }

            if (textDecoration != _lastTextDecoration)
            {
                // reset previous decoration
                WriteText(_lastTextDecoration switch
                {
                    TextDecorationLocation.Strikethrough => Esc.NoStrikethrough,
                    TextDecorationLocation.Underline => Esc.NoUnderline,
                    _ => String.Empty
                });

                // Add new decoration
                WriteText(textDecoration switch
                {
                    TextDecorationLocation.Underline => Esc.Underline,
                    TextDecorationLocation.Strikethrough => Esc.Strikethrough,
                    _ => String.Empty
                });
                _lastTextDecoration = textDecoration;
            }

            if (style != _lastStyle)
            {
                //reset previous style
                WriteText(_lastStyle switch
                {
                    FontStyle.Italic => Esc.NoItalic,
                    _ => String.Empty
                });

                WriteText(style switch
                {
                    FontStyle.Italic => Esc.Italic,
                    _ => String.Empty
                });
                _lastStyle = style;
            }

            if (weight != _lastWeight)
            {
                WriteText(weight switch
                {
                    FontWeight.Bold or FontWeight.SemiBold or FontWeight.ExtraBold or FontWeight.Black =>
                        Esc.Bold,
                    FontWeight.Thin or FontWeight.ExtraLight or FontWeight.Light => Esc.Dim,
                    _ => Esc.Normal
                });
                _lastWeight = weight;
            }

            if (foreground != _lastForeground || background != _lastBackground)
            {
                (object mappedBackground, object mappedForeground) =
                    consoleColorMode.Value.MapColors(background, foreground, weight);
                if (foreground != _lastForeground)
                {
                    WriteText(Esc.Foreground(mappedForeground));
                    _lastForeground = foreground;
                }

                if (background != _lastBackground)
                {
                    WriteText(Esc.Background(mappedBackground));
                    _lastBackground = background;
                }
            }

            // move to position
            if (SupportsEmojiVariation)
            {
                SetCaretPosition(bufferPoint);
                WriteText(str);
                ushort textWidth = str.MeasureText();
                bufferPoint = new PixelBufferCoordinate((ushort)(bufferPoint.X + textWidth), bufferPoint.Y);
            }
            else
            {
                // rendering over the top with the glyph.
                // process each glyph, rendering the width as spaces then moving the cursor and
                foreach (Grapheme grapheme in Grapheme.Parse(str, SupportsComplexEmoji))
                {
                    ushort glyphWidth = grapheme.Glyph.MeasureText();
                    if (glyphWidth > 1)
                    {
                        WriteText(Esc.SetCursorPosition(bufferPoint.X + 1, bufferPoint.Y));
                        WriteText(new string(' ', Math.Min(Size.Width - bufferPoint.X - 1, glyphWidth - 1)));
                    }

                    WriteText(Esc.SetCursorPosition(bufferPoint.X, bufferPoint.Y));
                    WriteText(grapheme.Glyph);

                    bufferPoint =
                        new PixelBufferCoordinate((ushort)(bufferPoint.X + glyphWidth), bufferPoint.Y);
                }
            }

            if (bufferPoint.X >= Size.Width)
            {
                bufferPoint = new PixelBufferCoordinate(0, (ushort)(bufferPoint.Y + 1));
            }
            _headBufferPoint = bufferPoint;
        }

        public void Flush()
        {
            if (_outputBuffer.Length > 0)
            {
                Debug.WriteLine(_outputBuffer.Length);
                Console.Write(_outputBuffer.ToString());
                _outputBuffer.Clear();
            }
        }

        /// <summary>
        ///     Write raw text to the console
        /// </summary>
        /// <remarks>This does not move the caret position, so should only be used for escape commands</remarks>
        /// <param name="str"></param>
        public void WriteText(string str)
        {
            _outputBuffer.Append(str);
        }

        public void PrepareConsole()
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.OutputEncoding = Encoding.UTF8;

            // enable alternate screen so original console screen is not affected by the app
            Console.Write(Esc.EnableAlternateBuffer);

            Size = new PixelBufferSize((ushort)Console.WindowWidth, (ushort)Console.WindowHeight);

            // Detect complex emoji support by writing a complex emoji and checking cursor position.
            // If the cursor moves 2 positions, it indicates proper rendering of composite surrogate pairs.
            (int left, _) = Console.GetCursorPosition();
            Console.Write(TestEmoji);
            (int left2, _) = Console.GetCursorPosition();
            _supportsComplexEmoji = left2 - left == 2;

            // write out a char with wide variation selector
            Console.Write("\U0001F5D1\uFE0F"); // 🗑 Wastebasket + emoji variation selector
            (int left3, _) = Console.GetCursorPosition();
            _supportsEmojiVariation = left3 - left2 == 2;

            ClearScreen();
            Flush();
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