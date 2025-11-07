using Avalonia.Media;
using AvaloniaEdit.Document;
using AvaloniaEdit.Rendering;

namespace Consolonia.AvaloniaEdit
{
    internal class DecorationsFontMetricsTransformer : DocumentColorizingTransformer
    {
        protected override void ColorizeLine(DocumentLine line)
        {
            ChangeLinePart(line.Offset, line.EndOffset, element =>
            {
                if (element.TextRunProperties?.TextDecorations != null)
                    foreach (TextDecoration decoration in element.TextRunProperties.TextDecorations)
                        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                        switch (decoration.Location)
                        {
                            case TextDecorationLocation.Underline:
                                decoration.StrokeThicknessUnit = TextDecorationUnit.Pixel;
                                decoration.StrokeThickness = element.TextRunProperties.Typeface.GlyphTypeface.Metrics
                                    .UnderlineThickness;
                                break;
                            case TextDecorationLocation.Strikethrough:
                                decoration.StrokeThicknessUnit = TextDecorationUnit.Pixel;
                                decoration.StrokeThickness = element.TextRunProperties.Typeface.GlyphTypeface.Metrics
                                    .StrikethroughThickness;
                                break;
                        }
            });
        }
    }
}