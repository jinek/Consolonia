//DUPFINDER_ignore
//todo: this file is under refactoring. Restore the duplication finder

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Consolonia.Controls.Brushes;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Consolonia.Core.Text;
using Consolonia.Core.Text.Fonts;
using SkiaSharp;

namespace Consolonia.Core.Drawing
{
    internal partial class DrawingContextImpl : IDrawingContextImpl
    {

        public void DrawBitmap(IBitmapImpl source, double opacity, Rect sourceRect, Rect destRect)
        {
            // resize bitmap to destination rect size
            var targetRect = new Rect(Transform.Transform(new Point(destRect.TopLeft.X, destRect.TopLeft.Y)),
                    Transform.Transform(new Point(destRect.BottomRight.X, destRect.BottomRight.Y)))
                .ToPixelRect();
            var bmp = (BitmapImpl)source;

            // resize source to be target rect * 2 so we can map to quad pixels
            using var bitmap = new SKBitmap(targetRect.Width * 2, targetRect.Height * 2);
            using var canvas = new SKCanvas(bitmap);
            using var skPaint = new SKPaint();
            skPaint.FilterQuality = SKFilterQuality.Medium;
            canvas.DrawBitmap(bmp.Bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), skPaint);

            // this is reused by each pixel as we process the bitmap
            Span<SKColor> quadPixelColors = stackalloc SKColor[4];

            int py = targetRect.TopLeft.Y;
            SKColor[] pixels = bitmap.Pixels;
            int pixelRow = 0;
            for (int y = 0; y < bitmap.Info.Height; y += 2, py++, pixelRow += 2 * bitmap.Width)
            {
                int px = targetRect.TopLeft.X;
                for (int x = 0; x < bitmap.Info.Width; x += 2, px++)
                {
                    var point = new PixelPoint(px, py);
                    if (CurrentClip.ContainsExclusive(point))
                    {
                        // get the quad pixel from the bitmap as a quad of 4 SKColor values
                        quadPixelColors[0] = pixels[pixelRow + x];
                        quadPixelColors[1] = pixels[pixelRow + x + 1];
                        quadPixelColors[2] = pixels[pixelRow + bitmap.Width + x];
                        quadPixelColors[3] = pixels[pixelRow + bitmap.Width + x + 1];

                        // map it to a single char to represent the 4 pixels
                        char quadPixelChar = GetQuadPixelCharacter(quadPixelColors);

                        // get the combined colors for the quad pixel
                        Color foreground = GetForegroundColorForQuadPixel(quadPixelChar, quadPixelColors);
                        Color background = GetBackgroundColorForQuadPixel(quadPixelChar, quadPixelColors);

                        var imagePixel = new Pixel(
                            new PixelForeground(new Symbol(quadPixelChar), foreground),
                            new PixelBackground(background));

                        _pixelBuffer[point] = _pixelBuffer[point].Blend(imagePixel);
                    }
                }
            }

            _consoleWindowImpl.DirtyRegions.AddRect(targetRect);
        }

        public void DrawBitmap(IBitmapImpl source, IBrush opacityMask, Rect opacityMaskRect, Rect destRect)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        ///     given 4 colors return quadPixel character which is suitable to represent the colors
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static char GetQuadPixelCharacter(Span<SKColor> colors)
        {
            char character = GetColorsPattern(colors) switch
            {
                // ReSharper disable StringLiteralTypo
                0b0000 => ' ',
                0b1000 => '▘',
                0b0100 => '▝',
                0b0010 => '▖',
                0b0001 => '▗',
                0b1001 => '▚',
                0b0110 => '▞',
                0b1010 => '▌',
                0b0101 => '▐',
                0b0011 => '▄',
                0b1100 => '▀',
                0b1110 => '▛',
                0b1101 => '▜',
                0b1011 => '▙',
                0b0111 => '▟',
                0b1111 => '█',
                // ReSharper restore StringLiteralTypo
                _ => throw new NotImplementedException()
            };
            return character;
        }


        /// <summary>
        ///     Combine the colors for the white part of the quad pixel character.
        /// </summary>
        /// <param name="quadPixel"></param>
        /// <param name="pixelColors">4 colors</param>
        /// <returns>foreground color</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Color GetForegroundColorForQuadPixel(char quadPixel, Span<SKColor> pixelColors)
        {
            if (pixelColors.Length != 4)
                throw new ArgumentException($"{nameof(pixelColors)} must have 4 elements.");

            SKColor skColor = quadPixel switch
            {
                ' ' => SKColors.Transparent,
                '▘' => pixelColors[0],
                '▝' => pixelColors[1],
                '▖' => pixelColors[2],
                '▗' => pixelColors[3],
                '▚' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[2] }),
                '▞' => CombineColors(stackalloc SKColor[] { pixelColors[1], pixelColors[3] }),
                '▌' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[2] }),
                '▐' => CombineColors(stackalloc SKColor[] { pixelColors[1], pixelColors[3] }),
                '▄' => CombineColors(stackalloc SKColor[] { pixelColors[2], pixelColors[3] }),
                '▀' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[1] }),
                '▛' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[1], pixelColors[2] }),
                '▜' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[1], pixelColors[3] }),
                '▙' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[2], pixelColors[3] }),
                '▟' => CombineColors(stackalloc SKColor[] { pixelColors[1], pixelColors[2], pixelColors[3] }),
                '█' => CombineColors(pixelColors),
                _ => throw new NotImplementedException()
            };

            return Color.FromRgb(skColor.Red, skColor.Green, skColor.Blue);
        }


        /// <summary>
        ///     Combine the colors for the black part of the quad pixel character.
        /// </summary>
        /// <param name="quadPixel"></param>
        /// <param name="pixelColors"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Color GetBackgroundColorForQuadPixel(char quadPixel, Span<SKColor> pixelColors)
        {
            SKColor skColor = quadPixel switch
            {
                ' ' => CombineColors(pixelColors),
                '▘' => CombineColors(stackalloc SKColor[] { pixelColors[1], pixelColors[2], pixelColors[3] }),
                '▝' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[2], pixelColors[3] }),
                '▖' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[1], pixelColors[3] }),
                '▗' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[1], pixelColors[2] }),
                '▚' => CombineColors(stackalloc SKColor[] { pixelColors[1], pixelColors[2] }),
                '▞' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[3] }),
                '▌' => CombineColors(stackalloc SKColor[] { pixelColors[1], pixelColors[3] }),
                '▐' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[2] }),
                '▄' => CombineColors(stackalloc SKColor[] { pixelColors[0], pixelColors[1] }),
                '▀' => CombineColors(stackalloc SKColor[] { pixelColors[2], pixelColors[3] }),
                '▛' => pixelColors[3],
                '▜' => pixelColors[2],
                '▙' => pixelColors[1],
                '▟' => pixelColors[0],
                '█' => SKColors.Transparent,
                _ => throw new NotImplementedException()
            };
            return Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue);
        }


        private static SKColor CombineColors(Span<SKColor> colors)
        {
            float accumR = 0, accumG = 0, accumB = 0;
            float accumAlpha = 0;

            foreach (ref readonly SKColor color in colors)
            {
                float a1 = color.Alpha / 255f;
                float oneMinusA = 1f - accumAlpha;

                accumR += color.Red * a1 * oneMinusA;
                accumG += color.Green * a1 * oneMinusA;
                accumB += color.Blue * a1 * oneMinusA;
                accumAlpha += a1 * oneMinusA;
            }

            byte r = (byte)Math.Clamp(accumR, 0, 255);
            byte g = (byte)Math.Clamp(accumG, 0, 255);
            byte b = (byte)Math.Clamp(accumB, 0, 255);
            byte a = (byte)Math.Clamp(accumAlpha * 255f, 0, 255);

            return new SKColor(r, g, b, a);
        }

        /// <summary>
        ///     Cluster quad colors into a pattern (like: TTFF) based on relative closeness
        /// </summary>
        /// <param name="colors"></param>
        /// <returns>T or F for each color as a string</returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte GetColorsPattern(Span<SKColor> colors)
        {
            if (colors.Length != 4) throw new ArgumentException("Array must contain exactly 4 colors.");

            // Initial guess: two clusters with the first two colors as centers
            Span<SKColor> clusterCenters = stackalloc SKColor[2] { colors[0], colors[1] };
            Span<SKColor> newClusterCenters = stackalloc SKColor[2];
            Span<int> clusters = stackalloc int[4];

            for (int iteration = 0; iteration < 10; iteration++) // limit iterations to avoid infinite loop
            {
                // Assign colors to the closest cluster center
                for (int i = 0; i < colors.Length; i++)
                    clusters[i] = GetColorCluster(colors[i], clusterCenters);

                // Recalculate cluster centers
                newClusterCenters[0] = SKColor.Empty;
                newClusterCenters[1] = SKColor.Empty;
                for (int cluster = 0; cluster < 2; cluster++)
                {
                    // Calculate average for this cluster 
                    int totalRed = 0, totalGreen = 0, totalBlue = 0, totalAlpha = 0;
                    int count = 0;
                    bool allTransparent = true;

                    for (int i = 0; i < colors.Length; i++)
                        if (clusters[i] == cluster)
                        {
                            SKColor color = colors[i];
                            totalRed += color.Red;
                            totalGreen += color.Green;
                            totalBlue += color.Blue;
                            totalAlpha += color.Alpha;
                            count++;

                            if (color.Alpha != 0)
                                allTransparent = false;
                        }

                    if (count > 0)
                        newClusterCenters[cluster] = new SKColor(
                            (byte)(totalRed / count),
                            (byte)(totalGreen / count),
                            (byte)(totalBlue / count),
                            (byte)(totalAlpha / count));

                    if (count == 4 && allTransparent)
                        return 0;
                }

                // Check for convergence
                bool converged = true;
                for (int i = 0; i < 2; i++)
                    if (!clusterCenters[i].Equals(newClusterCenters[i]))
                    {
                        converged = false;
                        break;
                    }

                if (converged)
                    break;

                clusterCenters[0] = newClusterCenters[0];
                clusterCenters[1] = newClusterCenters[1];
            }

            // Determine which cluster is lower and which is higher
            int lowerCluster = GetColorBrightness(clusterCenters[0]) < GetColorBrightness(clusterCenters[1]) ? 0 : 1;
            int higherCluster = 1 - lowerCluster;

            // represent bitmask where 0 for lower cluster and 1 for higher cluster
            return (byte)
                ((clusters[0] == higherCluster ? 0b1000 : 0) |
                 (clusters[1] == higherCluster ? 0b0100 : 0) |
                 (clusters[2] == higherCluster ? 0b0010 : 0) |
                 (clusters[3] == higherCluster ? 0b0001 : 0));
        }

        private static int GetColorCluster(SKColor color, Span<SKColor> clusterCenters)
        {
            double minDistance = double.MaxValue;
            int closestCluster = -1;

            for (int i = 0; i < clusterCenters.Length; i++)
            {
                double distance = GetColorDistance(color, clusterCenters[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCluster = i;
                }
            }

            return closestCluster;
        }

        private static double GetColorDistance(SKColor c1, SKColor c2)
        {
            int dr = c1.Red - c2.Red;
            int dg = c1.Green - c2.Green;
            int db = c1.Blue - c2.Blue;
            int da = c1.Alpha - c2.Alpha;

            return Math.Sqrt(dr * dr + dg * dg + db * db + da * da);
        }


        private static double GetColorBrightness(SKColor color)
        {
            return 0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue + color.Alpha;
        }
    }
}