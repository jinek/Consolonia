using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public interface IConsoleColorMode
    {
        Color Blend(Color color1, Color color2);

        (object background, object foreground) MapColors(Color background, Color foreground, FontWeight? weight);
    }
}