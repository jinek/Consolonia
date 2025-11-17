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
        /// <param name="context">drawingcontext to draw into</param>
        /// <param name="position">position to draw at</param>
        /// <param name="glyphRun">glyphrun to draw</param>
        /// <param name="foreground">color to use</param>
        /// <param name="rectToRefresh">returns the dirtyrect</param>
        void DrawGlyphRun(DrawingContextImpl context, PixelPoint position, GlyphRunImpl glyphRun,
            Color foreground, out PixelRect rectToRefresh);
    }
}