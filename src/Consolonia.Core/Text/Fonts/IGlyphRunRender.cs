using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Text.Fonts
{
    internal interface IGlyphRunRender
    {
        /// <summary>
        ///     Draw a glyph run using the specified drawing context at the specified position with the specified foreground color.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="position"></param>
        /// <param name="glyphRun"></param>
        /// <param name="foreground"></param>
        /// <returns>recttorefresh</returns>
        PixelRect DrawGlyphRun(DrawingContextImpl context, PixelPoint position, GlyphRunImpl glyphRun,
            Color foreground);
    }
}