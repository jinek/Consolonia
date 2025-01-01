using Avalonia.Media;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class RgbConsoleColorMode : IConsoleColorMode
    {
        public Color Blend(Color color1, Color color2)
        {
            // by chatGPT o1
            // Convert alpha from [0..255] to [0..1]
            float fgAlpha = color2.A / 255f;
            float bgAlpha = color1.A / 255f;

            // Compute output alpha
            float outAlpha = fgAlpha + bgAlpha * (1 - fgAlpha);

            // If there's no alpha in the result, return transparent
            if (outAlpha <= 0f) return Color.FromArgb(0, 0, 0, 0);

            // Calculate the composited color channels, also converting channels to [0..1]
            float outR = (color2.R / 255f * fgAlpha + color1.R / 255f * bgAlpha * (1 - fgAlpha)) / outAlpha;
            float outG = (color2.G / 255f * fgAlpha + color1.G / 255f * bgAlpha * (1 - fgAlpha)) / outAlpha;
            float outB = (color2.B / 255f * fgAlpha + color1.B / 255f * bgAlpha * (1 - fgAlpha)) / outAlpha;

            // Convert back to [0..255]
            byte a = (byte)(outAlpha * 255f);
            byte r = (byte)(outR * 255f);
            byte g = (byte)(outG * 255f);
            byte b = (byte)(outB * 255f);

            return Color.FromArgb(a, r, g, b);
        }

        public (object background, object foreground) MapColors(Color background, Color foreground, FontWeight? weight)
        {

            foreground = weight switch
            {
                FontWeight.Medium or FontWeight.SemiBold or FontWeight.Bold or FontWeight.ExtraBold or FontWeight.Black
                    or FontWeight.ExtraBlack
                    => foreground.Brighten(background),
                FontWeight.Thin or FontWeight.ExtraLight or FontWeight.Light
                    => foreground.Shade(background),
                _ => foreground
            };

            return (background, foreground);
        }
    }
}