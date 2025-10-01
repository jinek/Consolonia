using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Rendering;

namespace Consolonia.AvaloniaEdit
{
    public class ConsoleLineNumberMargin : LineNumberMargin
    {
        public override void Render(DrawingContext drawingContext)
        {
            //        base.Render(drawingContext);
            TextView textView = TextView;
            Size renderSize = Bounds.Size;

            if (textView is { VisualLinesValid: true })
            {
                IBrush foreground = GetValue(TextBlock.ForegroundProperty);
                foreach (VisualLine line in textView.VisualLines)
                {
                    int lineNumber = line.FirstDocumentLine.LineNumber;
                    double y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.LineTop);

                    var text = new FormattedText(lineNumber.ToString(CultureInfo.CurrentCulture),
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        Typeface.Default, 1, foreground);

                    drawingContext.DrawText(text,
                        new Point(renderSize.Width - text.Width, y - textView.VerticalOffset));
                }
            }
        }
    }
}