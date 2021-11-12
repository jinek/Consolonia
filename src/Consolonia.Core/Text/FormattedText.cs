﻿using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    // todo: Copypaste of avalonia skia. Probably must be simplified or re-used from skia
    internal class FormattedText : IFormattedTextImpl
    {
        private const float MAX_LINE_WIDTH = 10000; //todo: copied from avalonia, magicnumber
        private readonly List<FormattedTextLine> _lines = new();
        private readonly List<Rect> _rects = new();
        private readonly TextAlignment _textAlignment;
        private readonly TextWrapping _wrapping;
        private TypefaceImpl _glyphTypefaceImpl;
        private int _lineHeight;
        private int _lineOffset;

        public FormattedText(string? text, Typeface typeface, double fontSize, TextAlignment textAlignment,
            TextWrapping wrapping, Size constraint, IReadOnlyList<FormattedTextStyleSpan> spans)
        {
            Text = text ?? string.Empty;
            // Replace 0 characters with zero-width spaces (200B)
            Text = Text.Replace((char)0, (char)0x200B);

            _glyphTypefaceImpl = (TypefaceImpl)typeface.GlyphTypeface.PlatformImpl;

            _textAlignment = textAlignment;
            _wrapping = wrapping;
            Constraint = constraint;

            if (spans != null)
                foreach (FormattedTextStyleSpan span in spans)
                    if (span.ForegroundBrush != null)
                        SetForegroundBrush(span.ForegroundBrush, span.StartIndex, span.Length);

            Rebuild();
        }

        internal List<KeyValuePair<FBrushRange, IBrush>> ForegroundBrushes { get; } = new();
        internal List<AvaloniaFormattedTextLine> SkiaLines { get; private set; }


        public IEnumerable<FormattedTextLine> GetLines()
        {
            return _lines;
        }

        public TextHitTestResult HitTestPoint(Point point)
        {
            float y = (float)point.Y;

            AvaloniaFormattedTextLine line = default;

            float nextTop = 0;

            foreach (AvaloniaFormattedTextLine currentLine in SkiaLines)
                if (currentLine.Top <= y)
                {
                    line = currentLine;
                    nextTop = currentLine.Top + currentLine.Height;
                }
                else
                {
                    nextTop = currentLine.Top;
                    break;
                }

            if (!line.Equals(default(AvaloniaFormattedTextLine)))
            {
                var rects = GetRects();

                for (int c = line.Start; c < line.Start + line.TextLength; c++)
                {
                    Rect rc = rects[c];
                    if (rc.Contains(point))
                        return new TextHitTestResult
                        {
                            IsInside = !(line.TextLength > line.Length),
                            TextPosition = c,
                            IsTrailing = point.X - rc.X > rc.Width / 2
                        };
                }

                int offset = 0;

                if (point.X >= rects[line.Start].X + line.Width && line.Length > 0)
                    offset = line.TextLength > line.Length ? line.Length : line.Length - 1;

                if (y < nextTop)
                    return new TextHitTestResult
                    {
                        IsInside = false,
                        TextPosition = line.Start + offset,
                        IsTrailing = Text.Length == line.Start + offset + 1
                    };
            }

            bool end = point.X > Bounds.Width || point.Y > _lines.Sum(l => l.Height);

            return new TextHitTestResult
            {
                IsInside = false,
                IsTrailing = end,
                TextPosition = end ? Text.Length - 1 : 0
            };
        }

        public Rect HitTestTextPosition(int index)
        {
            const int caretWidth = 1;
            if (string.IsNullOrEmpty(Text))
            {
                float alignmentOffset = TransformX(0, 0);
                return new Rect(alignmentOffset, 0, caretWidth, _lineHeight);
            }

            var rects = GetRects();

            if (index < Text.Length && index >= 0) return rects[index];
            Rect r = rects.LastOrDefault();

            char c = Text[^1];

            switch (c)
            {
                case '\n':
                case '\r':
                    return new Rect(r.X, r.Y, caretWidth, _lineHeight);
                default:
                    return new Rect(r.X + r.Width, r.Y, caretWidth, _lineHeight);
            }
        }

        public IEnumerable<Rect> HitTestTextRange(int index, int length)
        {
            var result = new List<Rect>();

            var rects = GetRects();

            int lastIndex = index + length - 1;

            foreach (AvaloniaFormattedTextLine line in SkiaLines.Where(l =>
                l.Start + l.Length > index &&
                lastIndex >= l.Start))
            {
                int lineEndIndex = line.Start + (line.Length > 0 ? line.Length - 1 : 0);

                double left = rects[line.Start > index ? line.Start : index].X;
                double right = rects[lineEndIndex > lastIndex ? lastIndex : lineEndIndex].Right;

                result.Add(new Rect(left, line.Top, right - left, line.Height));
            }

            return result;
        }


        public Size Constraint { get; }
        public Rect Bounds { get; private set; }
        public string Text { get; }

        private void SetForegroundBrush(IBrush brush, int startIndex, int length)
        {
            var key = new FBrushRange(startIndex, length);
            int index = ForegroundBrushes.FindIndex(v => v.Key.Equals(key));

            if (index > -1) ForegroundBrushes.RemoveAt(index);

            if (brush != null)
            {
                brush = brush.ToImmutable();
                ForegroundBrushes.Insert(0, new KeyValuePair<FBrushRange, IBrush>(key, brush));
            }
        }


        private void Rebuild()
        {
            int length = Text.Length;

            _lines.Clear();
            _rects.Clear();
            SkiaLines = new List<AvaloniaFormattedTextLine>();

            int curOff = 0;
            float curY = 0;

            const int mLeading = 0; // The recommended distance to add between lines of text (will be >= 0).

            // This seems like the best measure of full vertical extent
            // matches Direct2D line height
            _lineHeight = 1;

            // Rendering is relative to baseline
            _lineOffset = 0;

            float widthConstraint = double.IsPositiveInfinity(Constraint.Width)
                ? -1
                : (float)Constraint.Width;

            while (curOff < length)
            {
                float constraint = -1;

                if (_wrapping == TextWrapping.Wrap)
                {
                    constraint = widthConstraint <= 0 ? MAX_LINE_WIDTH : widthConstraint;
                    if (constraint > MAX_LINE_WIDTH)
                        constraint = MAX_LINE_WIDTH;
                }

                int measured = LineBreak(Text, curOff, length, constraint, out int trailingnumber);
                var line = new AvaloniaFormattedTextLine
                {
                    Start = curOff,
                    TextLength = measured
                };
                string subString = Text.Substring(line.Start, line.TextLength);
                float lineWidth = subString.Length;
                line.Length = measured - trailingnumber;
                line.Width = lineWidth;
                line.Height = _lineHeight;
                line.Top = curY;

                SkiaLines.Add(line);

                curY += _lineHeight;
                curY += mLeading;
                curOff += measured;

                //if this is the last line and there are trailing newline characters then
                //insert a additional line
                if (curOff >= length)
                {
                    string subStringMinusNewlines = subString.TrimEnd('\n', '\r');
                    int lengthDiff = subString.Length - subStringMinusNewlines.Length;
                    if (lengthDiff <= 0) continue;
                    string lastLineSubString = Text.Substring(line.Start, line.TextLength);
                    int lastLineWidth = lastLineSubString.Length;
                    var lastLine = new AvaloniaFormattedTextLine
                    {
                        TextLength = lengthDiff,
                        Start = curOff - lengthDiff,
                        Length = 0,
                        Height = _lineHeight,
                        Top = curY,
                        Width = lastLineWidth
                    };

                    SkiaLines.Add(lastLine);

                    curY += _lineHeight;
                    curY += mLeading;
                }
            }

            // Now convert to Avalonia data formats
            _lines.Clear();
            float maxX = 0;

            for (int c = 0; c < SkiaLines.Count; c++)
            {
                float w = SkiaLines[c].Width;
                if (maxX < w)
                    maxX = w;

                _lines.Add(new FormattedTextLine(SkiaLines[c].TextLength, SkiaLines[c].Height));
            }

            if (SkiaLines.Count == 0)
            {
                _lines.Add(new FormattedTextLine(0, _lineHeight));
                Bounds = new Rect(0, 0, 0, _lineHeight);
            }
            else
            {
                AvaloniaFormattedTextLine lastLine = SkiaLines[SkiaLines.Count - 1];
                Bounds = new Rect(0, 0, maxX, lastLine.Top + lastLine.Height);

                if (double.IsPositiveInfinity(Constraint.Width)) return;

                switch (_textAlignment)
                {
                    case TextAlignment.Center:
                        Bounds = new Rect(Constraint).CenterRect(Bounds);
                        break;
                    case TextAlignment.Right:
                        Bounds = new Rect(
                            Constraint.Width - Bounds.Width,
                            0,
                            Bounds.Width,
                            Bounds.Height);
                        break;
                }
            }
        }

        private static int LineBreak(string textInput, int textIndex, int stop,
            float maxWidth,
            out int trailingCount)
        {
            int lengthBreak;
            if (maxWidth == -1)
                lengthBreak = stop - textIndex;
            else
                lengthBreak = (int)maxWidth;

            //Check for white space or line breakers before the lengthBreak
            int startIndex = textIndex;
            int index = textIndex;
            int word_start = textIndex;
            bool prevBreak = true;

            trailingCount = 0;

            while (index < stop)
            {
                int prevText = index;
                char currChar = textInput[index++];
                bool currBreak = IsBreakChar(currChar);

                if (!currBreak && prevBreak) word_start = prevText;

                prevBreak = currBreak;

                if (index > startIndex + lengthBreak)
                {
                    if (currBreak)
                    {
                        // eat the rest of the whitespace
                        while (index < stop && IsBreakChar(textInput[index])) index++;

                        trailingCount = index - prevText;
                    }
                    else
                    {
                        // backup until a whitespace (or 1 char)
                        if (word_start == startIndex)
                        {
                            if (prevText > startIndex) index = prevText;
                        }
                        else
                        {
                            index = word_start;
                        }
                    }

                    break;
                }

                if ('\n' == currChar)
                {
                    int ret = index - startIndex;
                    int lineBreakSize = 1;
                    if (index < stop)
                    {
                        currChar = textInput[index++];
                        if ('\r' == currChar)
                        {
                            ret = index - startIndex;
                            ++lineBreakSize;
                        }
                    }

                    trailingCount = lineBreakSize;

                    return ret;
                }

                if ('\r' == currChar)
                {
                    int ret = index - startIndex;
                    int lineBreakSize = 1;
                    if (index < stop)
                    {
                        currChar = textInput[index++];
                        if ('\n' == currChar)
                        {
                            ret = index - startIndex;
                            ++lineBreakSize;
                        }
                    }

                    trailingCount = lineBreakSize;

                    return ret;
                }
            }

            return index - startIndex;
        }

        private static bool IsBreakChar(char c)
        {
            //white space or zero space whitespace
            return char.IsWhiteSpace(c) || c == '\u200B';
        }

        internal float TransformX(float originX, float lineWidth)
        {
            TextAlignment align = _textAlignment;
            float x = 0;

            if (align == TextAlignment.Left)
            {
                x = originX;
            }
            else
            {
                double width = Constraint.Width > 0 && !double.IsPositiveInfinity(Constraint.Width)
                    ? Constraint.Width
                    : Bounds.Width;

                switch (align)
                {
                    case TextAlignment.Center:
                        x = originX + (float)(width - lineWidth) / 2;
                        break;
                    case TextAlignment.Right:
                        x = originX + (float)(width - lineWidth);
                        break;
                }
            }

            return x;
        }

        private List<Rect> GetRects()
        {
            if (Text.Length > _rects.Count) BuildRects();

            return _rects;
        }

        private void BuildRects()
        {
            // Build character rects

            for (int li = 0; li < SkiaLines.Count; li++)
            {
                AvaloniaFormattedTextLine line = SkiaLines[li];
                float prevRight = TransformX(0, line.Width);
                double nextTop = line.Top + line.Height;

                if (li + 1 < SkiaLines.Count) nextTop = SkiaLines[li + 1].Top;

                for (int i = line.Start; i < line.Start + line.TextLength; i++)
                {
                    float w = 1;

                    _rects.Add(new Rect(
                        prevRight,
                        line.Top,
                        w,
                        nextTop - line.Top));
                    prevRight += w;
                }
            }
        }

        internal struct AvaloniaFormattedTextLine
        {
            public float Height;
            public int Length;
            public int Start;
            public int TextLength;
            public float Top;
            public float Width;
        }

        internal struct FBrushRange
        {
            public FBrushRange(int startIndex, int length)
            {
                StartIndex = startIndex;
                Length = length;
            }

            public int EndIndex => StartIndex + Length;

            public int Length { get; }

            public int StartIndex { get; }

            public bool Intersects(int index, int len)
            {
                return index + len > StartIndex &&
                       StartIndex + Length > index;
            }

            public override string ToString()
            {
                return $"{StartIndex}-{EndIndex}";
            }
        }
    }
}