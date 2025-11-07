using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace Consolonia.AvaloniaEdit
{
    /// <summary>
    ///     Transformer that makes sure that glyphe metrics are used for underling and strikethrough decorations
    /// </summary>
    public class FontMetricsTransformer : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            // Process all elements in the line
            int lineStart = line.Offset;
            int lineEnd = line.EndOffset;

            ChangeLinePart(lineStart, lineEnd, element =>
            {
                // Check if the element has underline decoration
                if (element.TextRunProperties?.TextDecorations != null)
                    // Create a new decoration collection with FontRecommended
                    foreach (TextDecoration decoration in element.TextRunProperties.TextDecorations)
                        if (decoration.Location == TextDecorationLocation.Underline)
                        {
                            decoration.StrokeThicknessUnit = TextDecorationUnit.Pixel;
                            decoration.StrokeThickness = element.TextRunProperties.Typeface.GlyphTypeface.Metrics
                                .UnderlineThickness;
                        }
                        else if (decoration.Location == TextDecorationLocation.Strikethrough)
                        {
                            decoration.StrokeThicknessUnit = TextDecorationUnit.Pixel;
                            decoration.StrokeThickness = element.TextRunProperties.Typeface.GlyphTypeface.Metrics
                                .StrikethroughThickness;
                        }
            });
        }
    }
}