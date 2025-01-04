using System;
using System.Linq;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Consolonia.Controls
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class LineStyles
    {
        public static LineStyles Parse(string s)
        {
            return new LineStyles(s);
        }

        public LineStyles()
        {
            Left = Top = Right = Bottom = LineStyle.SingleLine;
        }

        public LineStyles(string text)
        {
            var styles = text.Split(' ', ',')
                .Select(token => Enum.Parse<LineStyle>(token))
                .ToList();

            Left = styles.Count > 0 ? styles[0] : LineStyle.SingleLine;
            Top = styles.Count > 1 ? styles[1] : Left;
            Right = styles.Count > 2 ? styles[2] : Top;
            Bottom = styles.Count > 3 ? styles[3] : Right;
        }

        public LineStyles(LineStyle? left = null, LineStyle? top = null, LineStyle? right = null, LineStyle? bottom = null)
        {
            Left = left ?? LineStyle.SingleLine;
            Top = top ?? Left;
            Right = right ?? Top;
            Bottom = bottom ?? Right;
        }

        public LineStyle Left { get; set; }
        public LineStyle Top { get; set; }
        public LineStyle Right { get; set; }
        public LineStyle Bottom { get; set; }

        public static implicit operator LineStyles(string text) => new LineStyles(text);
        
        public static implicit operator LineStyles(LineStyle lineStyle) => new LineStyles(lineStyle);

    }
}
