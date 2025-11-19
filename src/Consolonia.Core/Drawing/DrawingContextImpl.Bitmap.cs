//DUPFINDER_ignore
//todo: this file is under refactoring. Restore the duplication finder

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

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

            // Get the platform render interface to resize the bitmap
            IPlatformRenderInterface renderInterface = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();

            // Resize source to be target rect * 2 so we can map to quad pixels
            var targetSize = new PixelSize(targetRect.Width * 2, targetRect.Height * 2);
            using IBitmapImpl resizedBitmap = renderInterface.ResizeBitmap(source, targetSize, BitmapInterpolationMode.MediumQuality);

            // Get pixel data from the resized bitmap
            if (resizedBitmap is not IReadableBitmapImpl readableBitmap)
            {
                ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawGlyphRunNotSupported, this, source, opacity, sourceRect, destRect);
                return;
            }

            using ILockedFramebuffer frameBuffer = readableBitmap.Lock();

            int stride = frameBuffer.RowBytes;
            int bytesPerPixel = frameBuffer.Format.BitsPerPixel / 8;
            byte[] pixels = new byte[stride * frameBuffer.Size.Height];
            Marshal.Copy(frameBuffer.Address, pixels, 0, pixels.Length);

            // this is reused by each pixel as we process the bitmap
            Span<Color> quadPixelColors = stackalloc Color[4];

            int py = targetRect.TopLeft.Y;

            for (int y = 0; y < targetSize.Height; y += 2, py++)
            {
                int px = targetRect.TopLeft.X;
                for (int x = 0; x < targetSize.Width; x += 2, px++)
                {
                    var point = new PixelPoint(px, py);
                    if (CurrentClip.ContainsExclusive(point))
                    {
                        // get the quad pixel from the bitmap as a quad of 4 Color values
                        quadPixelColors[0] = GetPixelColor(pixels, x, y, stride, bytesPerPixel);
                        quadPixelColors[1] = GetPixelColor(pixels, x + 1, y, stride, bytesPerPixel);
                        quadPixelColors[2] = GetPixelColor(pixels, x, y + 1, stride, bytesPerPixel);
                        quadPixelColors[3] = GetPixelColor(pixels, x + 1, y + 1, stride, bytesPerPixel);

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Color GetPixelColor(byte[] pixels, int x, int y, int stride, int bytesPerPixel)
        {
            int offset = y * stride + x * bytesPerPixel;
            byte b = pixels[offset];
            byte g = pixels[offset + 1];
            byte r = pixels[offset + 2];
            byte a = bytesPerPixel == 4 ? pixels[offset + 3] : (byte)255;

            return Color.FromArgb(a, r, g, b);
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
        private static char GetQuadPixelCharacter(Span<Color> colors)
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
        private static Color GetForegroundColorForQuadPixel(char quadPixel, Span<Color> pixelColors)
        {
            if (pixelColors.Length != 4)
                throw new ArgumentException($"{nameof(pixelColors)} must have 4 elements.");

            Color color = quadPixel switch
            {
                ' ' => Colors.Transparent,
                '▘' => pixelColors[0],
                '▝' => pixelColors[1],
                '▖' => pixelColors[2],
                '▗' => pixelColors[3],
                '▚' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[2] }),
                '▞' => CombineColors(stackalloc Color[] { pixelColors[1], pixelColors[3] }),
                '▌' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[2] }),
                '▐' => CombineColors(stackalloc Color[] { pixelColors[1], pixelColors[3] }),
                '▄' => CombineColors(stackalloc Color[] { pixelColors[2], pixelColors[3] }),
                '▀' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[1] }),
                '▛' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[1], pixelColors[2] }),
                '▜' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[1], pixelColors[3] }),
                '▙' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[2], pixelColors[3] }),
                '▟' => CombineColors(stackalloc Color[] { pixelColors[1], pixelColors[2], pixelColors[3] }),
                '█' => CombineColors(pixelColors),
                _ => throw new NotImplementedException()
            };

            return color;
        }


        /// <summary>
        ///     Combine the colors for the black part of the quad pixel character.
        /// </summary>
        /// <param name="quadPixel"></param>
        /// <param name="pixelColors"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Color GetBackgroundColorForQuadPixel(char quadPixel, Span<Color> pixelColors)
        {
            Color color = quadPixel switch
            {
                ' ' => CombineColors(pixelColors),
                '▘' => CombineColors(stackalloc Color[] { pixelColors[1], pixelColors[2], pixelColors[3] }),
                '▝' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[2], pixelColors[3] }),
                '▖' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[1], pixelColors[3] }),
                '▗' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[1], pixelColors[2] }),
                '▚' => CombineColors(stackalloc Color[] { pixelColors[1], pixelColors[2] }),
                '▞' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[3] }),
                '▌' => CombineColors(stackalloc Color[] { pixelColors[1], pixelColors[3] }),
                '▐' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[2] }),
                '▄' => CombineColors(stackalloc Color[] { pixelColors[0], pixelColors[1] }),
                '▀' => CombineColors(stackalloc Color[] { pixelColors[2], pixelColors[3] }),
                '▛' => pixelColors[3],
                '▜' => pixelColors[2],
                '▙' => pixelColors[1],
                '▟' => pixelColors[0],
                '█' => Colors.Transparent,
                _ => throw new NotImplementedException()
            };
            return color;
        }


        private static Color CombineColors(Span<Color> colors)
        {
            float accumR = 0, accumG = 0, accumB = 0;
            float accumAlpha = 0;

            foreach (ref readonly Color color in colors)
            {
                float a1 = color.A / 255f;
                float oneMinusA = 1f - accumAlpha;

                accumR += color.R * a1 * oneMinusA;
                accumG += color.G * a1 * oneMinusA;
                accumB += color.B * a1 * oneMinusA;
                accumAlpha += a1 * oneMinusA;
            }

            byte r = (byte)Math.Clamp(accumR, 0, 255);
            byte g = (byte)Math.Clamp(accumG, 0, 255);
            byte b = (byte)Math.Clamp(accumB, 0, 255);
            byte a = (byte)Math.Clamp(accumAlpha * 255f, 0, 255);

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        ///     Cluster quad colors into a pattern (like: TTFF) based on relative closeness
        /// </summary>
        /// <param name="colors"></param>
        /// <returns>T or F for each color as a string</returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte GetColorsPattern(Span<Color> colors)
        {
            if (colors.Length != 4) throw new ArgumentException("Array must contain exactly 4 colors.");

            // Initial guess: two clusters with the first two colors as centers
            Span<Color> clusterCenters = stackalloc Color[2] { colors[0], colors[1] };
            Span<Color> newClusterCenters = stackalloc Color[2];
            Span<int> clusters = stackalloc int[4];

            for (int iteration = 0; iteration < 10; iteration++) // limit iterations to avoid infinite loop
            {
                // Assign colors to the closest cluster center
                for (int i = 0; i < colors.Length; i++)
                    clusters[i] = GetColorCluster(colors[i], clusterCenters);

                // Recalculate cluster centers
                newClusterCenters[0] = Colors.Transparent;
                newClusterCenters[1] = Colors.Transparent;
                for (int cluster = 0; cluster < 2; cluster++)
                {
                    // Calculate average for this cluster 
                    int totalRed = 0, totalGreen = 0, totalBlue = 0, totalAlpha = 0;
                    int count = 0;
                    bool allTransparent = true;

                    for (int i = 0; i < colors.Length; i++)
                        if (clusters[i] == cluster)
                        {
                            Color color = colors[i];
                            totalRed += color.R;
                            totalGreen += color.G;
                            totalBlue += color.B;
                            totalAlpha += color.A;
                            count++;

                            if (color.A != 0)
                                allTransparent = false;
                        }

                    if (count > 0)
                        newClusterCenters[cluster] = Color.FromArgb(
                            (byte)(totalAlpha / count),
                            (byte)(totalRed / count),
                            (byte)(totalGreen / count),
                            (byte)(totalBlue / count));

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

        private static int GetColorCluster(Color color, Span<Color> clusterCenters)
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

        private static double GetColorDistance(Color c1, Color c2)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            int da = c1.A - c2.A;

            return Math.Sqrt(dr * dr + dg * dg + db * db + da * da);
        }


        private static double GetColorBrightness(Color color)
        {
            return 0.299 * color.R + 0.587 * color.G + 0.114 * color.B + color.A;
        }
    }
}