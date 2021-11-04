using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    internal class FormattedText : IFormattedTextImpl
    {
        private readonly TextAlignment _textAlignment;
        private readonly TextWrapping _wrapping;
        private FormattedTextLine[] _lines;

        public FormattedText(string? text, Typeface typeface, double fontSize, TextAlignment textAlignment,
            TextWrapping wrapping, Size constraint, IReadOnlyList<FormattedTextStyleSpan> spans)
        {
            _textAlignment = textAlignment;
            _wrapping = wrapping;

            Text = text ?? string.Empty;

            // Replace 0 characters with zero-width spaces (200B)
            Text = Text.Replace((char)0, (char)0x200B);

            Constraint = constraint;
            if (_textAlignment != TextAlignment.Left) throw new NotImplementedException();
            //todo: other parameters
            //todo: spans

            Rebuild();
        }
        
        private const float MAX_LINE_WIDTH = 10000;//todo: copied from avalonia, does not make sense

        private void Rebuild()
        {
            /*
            var length = Text.Length;

            _lines.Clear();
            //_rects.Clear();
            //_skiaLines = new List<AvaloniaFormattedTextLine>();

            int curOff = 0;
            float curY = 0;

            string subString;

            float widthConstraint = double.IsPositiveInfinity(Constraint.Width)
                ? -1
                : (float)Constraint.Width;

            while (curOff < length)
            {
                float lineWidth = -1;
                int measured;
                int trailingnumber = 0;

                float constraint = -1;

                if (_wrapping == TextWrapping.Wrap)
                {
                    constraint = widthConstraint <= 0 ? MAX_LINE_WIDTH : widthConstraint;
                    if (constraint > MAX_LINE_WIDTH)
                        constraint = MAX_LINE_WIDTH;
                }

                measured = LineBreak(Text, curOff, length, _paint, constraint, out trailingnumber);
                AvaloniaFormattedTextLine line = new AvaloniaFormattedTextLine();
                line.StartConsolonia = curOff;
                line.TextLength = measured;
                subString = Text.Substring(line.StartConsolonia, line.TextLength);
                lineWidth = _paint.MeasureText(subString);
                line.Length = measured - trailingnumber;
                line.Width = lineWidth;
                line.Height = _lineHeight;
                line.Top = curY;

                _skiaLines.Add(line);

                curY += _lineHeight;
                curY += mLeading;
                curOff += measured;

                //if this is the last line and there are trailing newline characters then
                //insert a additional line
                if (curOff >= length)
                {
                    var subStringMinusNewlines = subString.TrimEnd('\n', '\r');
                    var lengthDiff = subString.Length - subStringMinusNewlines.Length;
                    if (lengthDiff > 0)
                    {
                        AvaloniaFormattedTextLine lastLine = new AvaloniaFormattedTextLine();
                        lastLine.TextLength = lengthDiff;
                        lastLine.StartConsolonia = curOff - lengthDiff;
                        var lastLineSubString = Text.Substring(line.StartConsolonia, line.TextLength);
                        var lastLineWidth = _paint.MeasureText(lastLineSubString);
                        lastLine.Length = 0;
                        lastLine.Width = lastLineWidth;
                        lastLine.Height = _lineHeight;
                        lastLine.Top = curY;

                        _skiaLines.Add(lastLine);

                        curY += _lineHeight;
                        curY += mLeading;
                    }
                }
            }

            // Now convert to Avalonia data formats
            _lines.Clear();
            float maxX = 0;

            for (var c = 0; c < _skiaLines.Count; c++)
            {
                var w = _skiaLines[c].Width;
                if (maxX < w)
                    maxX = w;

                _lines.Add(new FormattedTextLine(_skiaLines[c].TextLength, _skiaLines[c].Height));
            }

            if (_skiaLines.Count == 0)
            {
                _lines.Add(new FormattedTextLine(0, _lineHeight));
                _bounds = new Rect(0, 0, 0, _lineHeight);
            }
            else
            {
                var lastLine = _skiaLines[_skiaLines.Count - 1];
                _bounds = new Rect(0, 0, maxX, lastLine.Top + lastLine.Height);

                if (double.IsPositiveInfinity(Constraint.Width))
                {
                    return;
                }

                switch (_paint.TextAlign)
                {
                    case SKTextAlign.Center:
                        _bounds = new Rect(Constraint).CenterRect(_bounds);
                        break;
                    case SKTextAlign.Right:
                        _bounds = new Rect(
                            Constraint.Width - _bounds.Width,
                            0,
                            _bounds.Width,
                            _bounds.Height);
                        break;
                }
            }
        */
        }
        
        private static int LineBreak(string textInput, int textIndex, int stop,
            float maxWidth,
            out int trailingCount)
        {
            int lengthBreak;
            if (maxWidth == -1)
            {
                lengthBreak = stop - textIndex;
            }
            else
            {
                float measuredWidth;
                string subText = textInput.Substring(textIndex, stop - textIndex);
                throw new NotImplementedException("Need text break");
                //lengthBreak = (int)paint.BreakText(subText, maxWidth, out measuredWidth);
            }

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

                if (!currBreak && prevBreak)
                {
                    word_start = prevText;
                }

                prevBreak = currBreak;

                if (index > startIndex + lengthBreak)
                {
                    if (currBreak)
                    {
                        // eat the rest of the whitespace
                        while (index < stop && IsBreakChar(textInput[index]))
                        {
                            index++;
                        }

                        trailingCount = index - prevText;
                    }
                    else
                    {
                        // backup until a whitespace (or 1 char)
                        if (word_start == startIndex)
                        {
                            if (prevText > startIndex)
                            {
                                index = prevText;
                            }
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

        private static bool IsBreakChar(char currChar)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<FormattedTextLine> GetLines()
        {
            return _lines;
        }

        public TextHitTestResult HitTestPoint(Point point)
        {
            throw new NotImplementedException();
        }

        public Rect HitTestTextPosition(int index)
        {
            if (string.IsNullOrEmpty(Text))
            {
                float alignmentOffset = TransformX(0, 0);
                return new Rect(alignmentOffset, 0, 1, 1);
            }

            var r = new Rect(index, 0, 1, 1);
            if (index < Text.Length && index >= 0) return r;
            char c = Text[^1];

            switch (c)
            {
                case '\n':
                case '\r':
                    return new Rect(r.X - r.Width, r.Y, 1, 1);
                default:
                    return new Rect(r.X, r.Y, 1, 1);
            }
        }


        private float TransformX(float originX, float lineWidth)
        {
            float x = 0;

            if (_textAlignment == TextAlignment.Left)
            {
                x = originX;
            }
            else
            {
                /*double width = Constraint.Width > 0 && !double.IsPositiveInfinity(Constraint.Width) ?
                                Constraint.Width :
                                _simpleBounds.Width;

                switch (_textAlignment)
                {
                    case TextAlignment.Center: x = originX + (float)(width - lineWidth) / 2; break;
                    case TextAlignment.Right: x = originX + (float)(width - lineWidth); break;
                }*/
                throw new NotImplementedException();
            }

            return x;
        }

        public IEnumerable<Rect> HitTestTextRange(int index, int length)
        {
            //now only left positioned
            yield return new Rect(0, 0, length, 1);
            /*for (int i = 0; i < length; i++)
                yield return HitTestTextPosition(index);*/
        }

        public Size Constraint { get; }
        public Rect Bounds { get; }
        public string Text { get; }
    }
}