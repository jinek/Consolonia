using Avalonia.Media;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public interface IConsoleColorMode
    {
        Color Blend(Color color1, Color color2);
        void SetAttributes(InputLessDefaultNetConsole console, Color background, Color foreground, FontWeight? weight);
    }
}