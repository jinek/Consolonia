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
            var textView = TextView;
            var renderSize = Bounds.Size;

            if (textView is { VisualLinesValid: true })
            {
                var foreground = GetValue(TextBlock.ForegroundProperty);
                foreach (var line in textView.VisualLines)
                {
                    var lineNumber = line.FirstDocumentLine.LineNumber;
                    var y = line.GetTextLineVisualYPosition(line.TextLines[0], VisualYPosition.LineTop);

                    var text = new FormattedText(lineNumber.ToString(CultureInfo.CurrentCulture),
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        Typeface.Default, 1, foreground);

                    drawingContext.DrawText(text, new Point(renderSize.Width - text.Width, y - textView.VerticalOffset));
                }
            }
        }
    }
}
