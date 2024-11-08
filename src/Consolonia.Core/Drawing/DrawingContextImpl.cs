using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Consolonia.Core.Text;
using NeoSmart.Unicode;
using SkiaSharp;

namespace Consolonia.Core.Drawing
{
    internal class DrawingContextImpl : IDrawingContextImpl
    {
        private const byte VerticalStartPattern = 0b0010;
        private const byte VerticalLinePattern = 0b1010;
        private const byte VerticalEndPattern = 0b1000;
        private const byte HorizontalStartPattern = 0b0100;
        private const byte HorizontalLinePattern = 0b0101;
        private const byte HorizontalEndPattern = 0b0001;

        public const int UnderlineThickness = 10;
        public const int StrikethroughThickness = 11;

        private readonly Stack<Rect> _clipStack = new(100);
        private readonly ConsoleWindow _consoleWindow;
        private readonly PixelBuffer _pixelBuffer;
        private readonly Matrix _postTransform = Matrix.Identity;
        private Matrix _transform = Matrix.Identity;

        public DrawingContextImpl(ConsoleWindow consoleWindow, PixelBuffer pixelBuffer)
        {
            _consoleWindow = consoleWindow;
            _pixelBuffer = pixelBuffer;
            _clipStack.Push(pixelBuffer.Size);
        }

        private Rect CurrentClip => _clipStack.Peek();

        public void Dispose()
        {
        }

        public void Clear(Color color)
        {
            /*if (color != Colors.Transparent)
            {
                ConsoloniaPlatform.RaiseNotSupported(1);
                return;
            }

            _pixelBuffer.Foreach((_, _, _) =>
                new Pixel(new PixelBackground()));*/
        }

        public void DrawBitmap(IBitmapImpl source, double opacity, Rect sourceRect, Rect destRect)
        {
            // resize bitmap to destination rect size
            var targetRect = new Rect(Transform.Transform(new Point(destRect.TopLeft.X, destRect.TopLeft.Y)),
                Transform.Transform(new Point(destRect.BottomRight.X, destRect.BottomRight.Y)));
            var bmp = (BitmapImpl)source;

            // resize source to be target rect * 2 so we can map to quad pixels
            using var bitmap = new SKBitmap((int)targetRect.Width * 2, (int)targetRect.Height * 2);
            using var canvas = new SKCanvas(bitmap);
            canvas.DrawBitmap(bmp.Bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height),
                new SKPaint { FilterQuality = SKFilterQuality.Medium });

            for (int y = 0; y < bitmap.Info.Height; y += 2)
                for (int x = 0; x < bitmap.Info.Width; x += 2)
                {
                    // NOTE: we divide by 2 because we are working with quad pixels,
                    // // the bitmap has twice the horizontal and twice the vertical of the target rect.
                    int px = (int)targetRect.TopLeft.X + x / 2;
                    int py = (int)targetRect.TopLeft.Y + y / 2;

                    // get the quad pixel the bitmap
                    var quadColors = new[]
                    {
                    bitmap.GetPixel(x, y), bitmap.GetPixel(x + 1, y),
                    bitmap.GetPixel(x, y + 1), bitmap.GetPixel(x + 1, y + 1)
                };

                    // map it to a single char to represet the 4 pixels
                    char quadPixel = GetQuadPixelCharacter(quadColors);

                    // get the combined colors for the quad pixel
                    Color foreground = GetForegroundColorForQuadPixel(quadColors, quadPixel);
                    Color background = GetBackgroundColorForQuadPixel(quadColors, quadPixel);

                    var imagePixel = new Pixel(
                        new PixelForeground(new SimpleSymbol(quadPixel), color: foreground),
                        new PixelBackground(background));
                    CurrentClip.ExecuteWithClipping(new Point(px, py),
                        () =>
                        {
                            _pixelBuffer.Set(new PixelBufferCoordinate((ushort)px, (ushort)py),
                                (existingPixel, _) => existingPixel.Blend(imagePixel), imagePixel.Background.Color);
                        });
                }
        }

        public void DrawBitmap(IBitmapImpl source, IBrush opacityMask, Rect opacityMaskRect, Rect destRect)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(IPen pen, Point p1, Point p2)
        {
            DrawLineInternal(pen, Line.CreateMyLine(p1, p2));
        }

        public void DrawGeometry(IBrush brush, IPen pen, IGeometryImpl geometry)
        {
            switch (geometry)
            {
                case Rectangle myRectangle:
                    DrawRectangle(brush, pen, new RoundedRect(myRectangle.Rect));
                    break;
                case Line myLine:
                    DrawLineInternal(pen, myLine);
                    break;
                default:
                    ConsoloniaPlatform.RaiseNotSupported(5);
                    break;
            }
        }

        public void DrawRectangle(IBrush brush, IPen pen, RoundedRect rect, BoxShadows boxShadows = new())
        {
            if (brush == null && pen == null) return; //this is simple Panel for example

            if (rect.IsRounded || !rect.IsUniform)
            {
                ConsoloniaPlatform.RaiseNotSupported(10);
                return;
            }

            if (boxShadows.Count > 0) throw new NotImplementedException();

            if (rect.Rect.IsEmpty()) return;
            Rect r = rect.Rect;

            if (brush is not null)
            {
                switch (brush)
                {
                    case VisualBrush:
                        throw new NotImplementedException();
                    case ISceneBrush sceneBrush:
                        {
                            ISceneBrushContent sceneBrushContent = sceneBrush.CreateContent();
                            if (sceneBrushContent != null) sceneBrushContent.Render(this, Matrix.Identity);
                            return;
                        }
                }

                Rect r2 = r.TransformToAABB(Transform);

                double width = r2.Width + (pen?.Thickness ?? 0);
                double height = r2.Height + (pen?.Thickness ?? 0);
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                    {
                        int px = (int)(r2.TopLeft.X + x);
                        int py = (int)(r2.TopLeft.Y + y);

                        ConsoleBrush backgroundBrush = ConsoleBrush.FromPosition(brush, x, y, (int)width, (int)height);
                        CurrentClip.ExecuteWithClipping(new Point(px, py), () =>
                        {
                            _pixelBuffer.Set(new PixelBufferCoordinate((ushort)px, (ushort)py),
                                (pixel, bb) => pixel.Blend(new Pixel(new PixelBackground(bb.Mode, bb.Color))),
                                backgroundBrush);
                        });
                    }
            }

            if (pen is null or { Thickness: 0 }
                or { Brush: null }) return;

            DrawRectangleLineInternal(pen, new Line(r.TopLeft, false, (int)r.Width));
            DrawRectangleLineInternal(pen, new Line(r.BottomLeft, false, (int)r.Width));
            DrawRectangleLineInternal(pen, new Line(r.TopLeft, true, (int)r.Height));
            DrawRectangleLineInternal(pen, new Line(r.TopRight, true, (int)r.Height));
        }

        public void DrawEllipse(IBrush brush, IPen pen, Rect rect)
        {
            throw new NotImplementedException();
        }

        public void DrawGlyphRun(IBrush foreground, IGlyphRunImpl glyphRun)
        {
            if (glyphRun is not GlyphRunImpl glyphRunImpl)
            {
                ConsoloniaPlatform.RaiseNotSupported(17, glyphRun);
                throw new InvalidProgramException();
            }

            if (glyphRun.FontRenderingEmSize.IsNearlyEqual(0)) return;
            if (!glyphRun.FontRenderingEmSize.IsNearlyEqual(1))
            {
                ConsoloniaPlatform.RaiseNotSupported(3);
                return;
            }

            var shapedBuffer = (ShapedBuffer)glyphRunImpl.GlyphInfos;
            var text = shapedBuffer.Text.ToString();
            DrawStringInternal(foreground, text, glyphRun.GlyphTypeface);
        }

        public IDrawingContextLayerImpl CreateLayer(Size size)
        {
            return new RenderTarget(_consoleWindow);
        }

        public void PushClip(Rect clip)
        {
            clip = new Rect(clip.Position.Transform(Transform), clip.BottomRight.Transform(Transform));
            _clipStack.Push(CurrentClip.Intersect(clip));
        }

        public void PushClip(RoundedRect clip)
        {
            if (clip.IsRounded)
                ConsoloniaPlatform.RaiseNotSupported(2);

            PushClip(clip.Rect);
        }

        public void PopClip()
        {
            _clipStack.Pop();
        }

        public void PushOpacity(double opacity, Rect? bounds)
        {
            if (opacity.IsNearlyEqual(1)) return;
            ConsoloniaPlatform.RaiseNotSupported(7);
        }

        public void PopOpacity()
        {
            throw new NotImplementedException();
        }

        public void PushOpacityMask(IBrush mask, Rect bounds)
        {
            throw new NotImplementedException();
        }

        public void PopOpacityMask()
        {
            throw new NotImplementedException();
        }

        public void PushGeometryClip(IGeometryImpl clip)
        {
            if (clip is not Rectangle myRectangle) throw new NotImplementedException();
            PushClip(myRectangle.Rect);
        }

        public void PopGeometryClip()
        {
            PopClip();
        }

        public void PushRenderOptions(RenderOptions renderOptions)
        {
            throw new NotImplementedException();
        }

        public void PopRenderOptions()
        {
            throw new NotImplementedException();
        }

        public object GetFeature(Type t)
        {
            throw new NotImplementedException();
        }

        public RenderOptions RenderOptions { get; set; }

        public Matrix Transform
        {
            get => _transform;
            set => _transform = value * _postTransform;
        }

        /// <summary>
        ///     Draw a straight horizontal line or vertical line
        /// </summary>
        /// <param name="pen">pen</param>
        /// <param name="line">line</param>
        private void DrawLineInternal(IPen pen, Line line)
        {
            if (pen.Thickness == 0) return;

            line = TransformLineInternal(line);

            Point head = line.PStart;

            if (IfMoveConsoleCaretMove(pen, head))
                return;

            if (line.Vertical == false && pen.Thickness > 1)
            {
                // horizontal lines with thickness larger than one are text decorations
                ApplyTextDecorationLineInternal(ref head, pen, line);
                return;
            }

            var extractColorCheckPlatformSupported = ExtractColorOrNullWithPlatformCheck(pen, out var lineStyle);
            if (extractColorCheckPlatformSupported == null)
                return;

            var color = (Color)extractColorCheckPlatformSupported;

            byte pattern = (byte)(line.Vertical ? 0b1010 : 0b0101);
            DrawPixelAndMoveHead(ref head, line, lineStyle, pattern, color, line.Length); //line
        }

        private void ApplyTextDecorationLineInternal(ref Point head, IPen pen, Line line)
        {
            TextDecorationCollection textDecoration = pen.Thickness switch
            {
                UnderlineThickness => TextDecorations.Underline,
                StrikethroughThickness => TextDecorations.Strikethrough,
                _ => throw new ArgumentOutOfRangeException($"Unsupported thickness {pen.Thickness}")
            };

            for (int x = 0; x < line.Length; x++)
            {
                Point h = head;
                CurrentClip.ExecuteWithClipping(h, () =>
                {
                    // ReSharper disable once AccessToModifiedClosure todo: pass as a parameter
                    _pixelBuffer.Set((PixelBufferCoordinate)h,
                        pixel =>
                        {
                            var newPixelForeground = new PixelForeground(pixel.Foreground.Symbol,
                                pixel.Foreground.Weight,
                                pixel.Foreground.Style,
                                textDecoration,
                                pixel.Foreground.Color);
                            return pixel.Blend(new Pixel(newPixelForeground, pixel.Background));
                        });
                });
                head = head.WithX(head.X + 1);
            }
        }

        /// <summary>
        ///     Draw a rectangle line with corners
        /// </summary>
        /// <param name="pen">pen</param>
        /// <param name="line">line</param>
        private void DrawRectangleLineInternal(IPen pen, Line line)
        {
            if (pen.Thickness == 0) return;

            line = TransformLineInternal(line);

            Point head = line.PStart;

            if (IfMoveConsoleCaretMove(pen, head))
                return;

            var extractColorCheckPlatformSupported = ExtractColorOrNullWithPlatformCheck(pen, out var lineStyle);
            if (extractColorCheckPlatformSupported == null)
                return;

            var color = (Color)extractColorCheckPlatformSupported;

            byte pattern = line.Vertical ? VerticalStartPattern : HorizontalStartPattern;
            DrawPixelAndMoveHead(ref head, line, lineStyle, pattern, color, 1); //beginning

            pattern = line.Vertical ? VerticalLinePattern : HorizontalLinePattern;
            DrawPixelAndMoveHead(ref head, line, lineStyle, pattern, color, line.Length - 1); //line

            pattern = line.Vertical ? VerticalEndPattern : HorizontalEndPattern;
            DrawPixelAndMoveHead(ref head, line, lineStyle, pattern, color, 1); //ending 
        }

        /// <summary>
        ///     Transform line coordinates
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private Line TransformLineInternal(Line line)
        {
            if (!Transform.NoRotation()) ConsoloniaPlatform.RaiseNotSupported(16);

            line = (Line)line.WithTransform(Transform);
            return line;
        }

        /// <summary>
        ///     If the pen brush is a MoveConsoleCaretToPositionBrush, move the caret
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        private bool IfMoveConsoleCaretMove(IPen pen, Point head)
        {
            if (pen.Brush is not MoveConsoleCaretToPositionBrush)
                return false;

            _pixelBuffer.SetCaretPosition((PixelBufferCoordinate)head);
            return true;
        }

        /// <summary>
        ///     Extract color from pen brush
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="lineStyle"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static Color? ExtractColorOrNullWithPlatformCheck(IPen pen, out LineStyle? lineStyle)
        {
            lineStyle = null;
            if (pen is not
                {
                    Brush: ConsoleBrush or LineBrush or ImmutableSolidColorBrush,
                    // Thickness: 1,
                    DashStyle: null or { Dashes: { Count: 0 } },
                    LineCap: PenLineCap.Flat,
                    LineJoin: PenLineJoin.Miter
                })
            {
                ConsoloniaPlatform.RaiseNotSupported(6);
                return null;
            }

            if (pen.Brush is LineBrush lineBrush)
                lineStyle = lineBrush.LineStyle;

            ConsoleBrush consoleBrush = ConsoleBrush.FromBrush(pen.Brush);

            switch (consoleBrush.Mode)
            {
                case PixelBackgroundMode.Colored:
                    return consoleBrush.Color;
                case PixelBackgroundMode.Transparent:
                    return null;
                case PixelBackgroundMode.Shaded:
                    ConsoloniaPlatform.RaiseNotSupported(8);
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pen));
            }
        }

        /// <summary>
        ///     Draw pixels for a line with linestyle and a pattern
        /// </summary>
        /// <param name="head">the current caret position</param>
        /// <param name="line">line to render</param>
        /// <param name="lineStyle">line style</param>
        /// <param name="pattern">pattern of character to use</param>
        /// <param name="color">color for char</param>
        /// <param name="count">number of chars</param>
        private void DrawPixelAndMoveHead(ref Point head, Line line, LineStyle? lineStyle, byte pattern, Color color,
            int count)
        {
            for (int i = 0; i < count; i++)
            {
                Point h = head;
                CurrentClip.ExecuteWithClipping(h, () =>
                {
                    // ReSharper disable once AccessToModifiedClosure todo: pass as a parameter
                    _pixelBuffer.Set((PixelBufferCoordinate)h,
                        (pixel, mcC) => pixel.Blend(new Pixel(DrawingBoxSymbol.UpRightDownLeftFromPattern(
                            mcC.pattern,
                            lineStyle ?? LineStyle.SingleLine), mcC.consoleColor)),
                        (pattern, consoleColor: color));
                });
                head = line.Vertical
                    ? head.WithY(head.Y + 1)
                    : head.WithX(head.X + 1);
            }
        }

        private void DrawStringInternal(IBrush foreground, string text, IGlyphTypeface typeface, Point origin = new())
        {
            foreground = ConsoleBrush.FromBrush(foreground);
            if (foreground is not ConsoleBrush { Mode: PixelBackgroundMode.Colored } consoleBrush)
            {
                ConsoloniaPlatform.RaiseNotSupported(4);
                return;
            }

            // if (!Transform.IsTranslateOnly()) ConsoloniaPlatform.RaiseNotSupported(15);

            Point whereToDraw = origin.Transform(Transform);
            int currentXPosition = 0;
            int currentYPosition = 0;

            // Process text into collection of glyphs where
            // a glyph is either text or a combination of chars which make up an emoji.
            List<string> glyphs = new List<string>();
            StringBuilder emoji = new StringBuilder();
            var runes = text.EnumerateRunes();
            Rune lastRune = new Rune();

            while (runes.MoveNext())
            {
                if (lastRune.Value == Codepoints.ZWJ ||
                    lastRune.Value == Codepoints.ORC ||
                    Emoji.IsEmoji(runes.Current.ToString()))
                {
                    emoji.Append(runes.Current);
                }
                else if (runes.Current.Value == Emoji.ZeroWidthJoiner ||
                        runes.Current.Value == Emoji.ObjectReplacementCharacter ||
                        runes.Current.Value == Codepoints.VariationSelectors.EmojiSymbol ||
                        runes.Current.Value == Codepoints.VariationSelectors.TextSymbol)
                {
                    emoji.Append(runes.Current);
                }
                else
                {
                    if (emoji.Length > 0)
                    {
                        glyphs.Add(emoji.ToString());
                        emoji.Clear();
                    }
                    glyphs.Add(runes.Current.ToString());
                }
                lastRune = runes.Current;
            }

            // Each glyph maps to a pixel as a starting point.
            // Emoji's and Ligatures are complex strings, so they start at a point and then overlap following pixels
            // the x and y are adjusted accodingly.
            foreach (var glyph in glyphs)
            {
                Point characterPoint = whereToDraw.Transform(Matrix.CreateTranslation(currentXPosition, currentYPosition));
                Color foregroundColor = consoleBrush.Color;

                switch (glyph)
                {
                    case "\t":
                        {
                            const int tabSize = 8;
                            var consolePixel = new Pixel(new SimpleSymbol(' '), foregroundColor);
                            for (int j = 0; j < tabSize; j++)
                            {
                                Point newCharacterPoint = characterPoint.WithX(characterPoint.X + j);
                                CurrentClip.ExecuteWithClipping(newCharacterPoint, () =>
                                {
                                    _pixelBuffer.Set((PixelBufferCoordinate)newCharacterPoint,
                                        (oldPixel, cp) => oldPixel.Blend(cp), consolePixel);
                                });
                            }

                            currentXPosition += tabSize - 1;
                        }
                        break;
                    case "\r":
                    case "\f":
                    case "\n":
                        currentXPosition = 0;
                        currentYPosition++;
                        break;
                    default:
                        {
                            var symbol = new SimpleSymbol(glyph);
                            var consolePixel = new Pixel(symbol, foregroundColor, typeface.Style, typeface.Weight);
                            CurrentClip.ExecuteWithClipping(characterPoint, () =>
                            {
                                _pixelBuffer.Set((PixelBufferCoordinate)characterPoint,
                                    (oldPixel, cp) => oldPixel.Blend(cp), consolePixel);
                            });

                            if (symbol.Width > 1)
                                currentXPosition += symbol.Width;
                            else
                                currentXPosition++;
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///     given 4 colors return quadPixel character which is suitable to represent the colors
        /// </summary>
        /// <param name="colors"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static char GetQuadPixelCharacter(params SKColor[] colors)
        {
            char character = GetColorsPattern(colors) switch
            {
                "FFFF" => ' ',
                "TFFF" => '▘',
                "FTFF" => '▝',
                "FFTF" => '▖',
                "FFFT" => '▗',
                "TFFT" => '▚',
                "FTTF" => '▞',
                "TFTF" => '▌',
                "FTFT" => '▐',
                "FFTT" => '▄',
                "TTFF" => '▀',
                "TTTF" => '▛',
                "TTFT" => '▜',
                "TFTT" => '▙',
                "FTTT" => '▟',
                "TTTT" => '█',
                _ => throw new NotImplementedException()
            };
            return character;
        }


        /// <summary>
        ///     Combine the colors for the white part of the quad pixel character.
        /// </summary>
        /// <param name="pixelColors">4 colors</param>
        /// <param name="quadPixel"></param>
        /// <returns>foreground color</returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Color GetForegroundColorForQuadPixel(SKColor[] pixelColors, char quadPixel)
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
                '▚' => CombineColors(pixelColors[0], pixelColors[2]),
                '▞' => CombineColors(pixelColors[1], pixelColors[3]),
                '▌' => CombineColors(pixelColors[0], pixelColors[2]),
                '▐' => CombineColors(pixelColors[1], pixelColors[3]),
                '▄' => CombineColors(pixelColors[2], pixelColors[3]),
                '▀' => CombineColors(pixelColors[0], pixelColors[1]),
                '▛' => CombineColors(pixelColors[0], pixelColors[1], pixelColors[2]),
                '▜' => CombineColors(pixelColors[0], pixelColors[1], pixelColors[3]),
                '▙' => CombineColors(pixelColors[0], pixelColors[2], pixelColors[3]),
                '▟' => CombineColors(pixelColors[1], pixelColors[2], pixelColors[3]),
                '█' => CombineColors(pixelColors.ToArray()),
                _ => throw new NotImplementedException()
            };

            return Color.FromRgb(skColor.Red, skColor.Green, skColor.Blue);
        }


        /// <summary>
        ///     Combine the colors for the black part of the quad pixel character.
        /// </summary>
        /// <param name="pixelColors"></param>
        /// <param name="quadPixel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Color GetBackgroundColorForQuadPixel(SKColor[] pixelColors, char quadPixel)
        {
            SKColor skColor = quadPixel switch
            {
                ' ' => CombineColors(pixelColors.ToArray()),
                '▘' => CombineColors(pixelColors[1], pixelColors[2], pixelColors[3]),
                '▝' => CombineColors(pixelColors[0], pixelColors[2], pixelColors[3]),
                '▖' => CombineColors(pixelColors[0], pixelColors[1], pixelColors[3]),
                '▗' => CombineColors(pixelColors[0], pixelColors[1], pixelColors[2]),
                '▚' => CombineColors(pixelColors[1], pixelColors[2]),
                '▞' => CombineColors(pixelColors[0], pixelColors[3]),
                '▌' => CombineColors(pixelColors[1], pixelColors[3]),
                '▐' => CombineColors(pixelColors[0], pixelColors[2]),
                '▄' => CombineColors(pixelColors[0], pixelColors[1]),
                '▀' => CombineColors(pixelColors[2], pixelColors[3]),
                '▛' => pixelColors[3],
                '▜' => pixelColors[2],
                '▙' => pixelColors[1],
                '▟' => pixelColors[0],
                '█' => SKColors.Transparent,
                _ => throw new NotImplementedException()
            };
            return Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue);
        }

        //private static SKColor CombineColors(params SKColor[] colors)
        //{
        //    return new SKColor((byte)colors.Average(c => c.Red),
        //                       (byte)colors.Average(c => c.Green),
        //                       (byte)colors.Average(c => c.Blue),
        //                       (byte)colors.Average(c => c.Alpha));
        //}

        private static SKColor CombineColors(params SKColor[] colors)
        {
            float finalRed = 0;
            float finalGreen = 0;
            float finalBlue = 0;
            float finalAlpha = 0;

            foreach (SKColor color in colors)
            {
                float alphaRatio = color.Alpha / 255.0f;
                finalRed = (finalRed * finalAlpha + color.Red * alphaRatio) / (finalAlpha + alphaRatio);
                finalGreen = (finalGreen * finalAlpha + color.Green * alphaRatio) / (finalAlpha + alphaRatio);
                finalBlue = (finalBlue * finalAlpha + color.Blue * alphaRatio) / (finalAlpha + alphaRatio);
                finalAlpha += alphaRatio * (1 - finalAlpha);
            }

            byte red = (byte)Math.Clamp(finalRed, 0, 255);
            byte green = (byte)Math.Clamp(finalGreen, 0, 255);
            byte blue = (byte)Math.Clamp(finalBlue, 0, 255);
            byte alpha = (byte)Math.Clamp(finalAlpha * 255, 0, 255);

            return new SKColor(red, green, blue, alpha);
        }

        /// <summary>
        ///     Cluster quad colors into a pattern (like: TTFF) based on relative closeness
        /// </summary>
        /// <param name="colors"></param>
        /// <returns>T or F for each color as a string</returns>
        /// <exception cref="ArgumentException"></exception>
        private static string GetColorsPattern(SKColor[] colors)
        {
            if (colors.Length != 4) throw new ArgumentException("Array must contain exactly 4 colors.");

            // Initial guess: two clusters with the first two colors as centers
            SKColor[] clusterCenters = { colors[0], colors[1] };
            int[] clusters = new int[colors.Length];

            for (int iteration = 0; iteration < 10; iteration++) // limit iterations to avoid infinite loop
            {
                // Assign colors to the closest cluster center
                for (int i = 0; i < colors.Length; i++) clusters[i] = GetColorCluster(colors[i], clusterCenters);

                // Recalculate cluster centers
                var newClusterCenters = new SKColor[2];
                for (int cluster = 0; cluster < 2; cluster++)
                {
                    var clusteredColors = colors.Where((_, i) => clusters[i] == cluster).ToList();
                    if (clusteredColors.Any())
                        newClusterCenters[cluster] = GetAverageColor(clusteredColors);
                    if (clusteredColors.Count == 4)
                        if (clusteredColors.All(c => c.Alpha == 0))
                            return "FFFF";
                    //    return "TTTT";
                }

                // Check for convergence
                if (newClusterCenters.SequenceEqual(clusterCenters))
                    break;

                clusterCenters = newClusterCenters;
            }

            // Determine which cluster is lower and which is higher
            int lowerCluster = GetColorBrightness(clusterCenters[0]) < GetColorBrightness(clusterCenters[1]) ? 0 : 1;
            int higherCluster = 1 - lowerCluster;

            // Replace colors with 0 for lower cluster and 1 for higher cluster
            var sb = new StringBuilder();
            for (int i = 0; i < colors.Length; i++) sb.Append(clusters[i] == higherCluster ? 'T' : 'F');

            return sb.ToString();
        }

        private static int GetColorCluster(SKColor color, SKColor[] clusterCenters)
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
            return Math.Sqrt(
                Math.Pow(c1.Red - c2.Red, 2) +
                Math.Pow(c1.Green - c2.Green, 2) +
                Math.Pow(c1.Blue - c2.Blue, 2) +
                Math.Pow(c1.Alpha - c2.Alpha, 2)
            );
        }

        private static SKColor GetAverageColor(List<SKColor> colors)
        {
            byte averageRed = (byte)colors.Average(c => c.Red);
            byte averageGreen = (byte)colors.Average(c => c.Green);
            byte averageBlue = (byte)colors.Average(c => c.Blue);
            byte averageAlpha = (byte)colors.Average(c => c.Alpha);

            return new SKColor(averageRed, averageGreen, averageBlue, averageAlpha);
        }

        private static double GetColorBrightness(SKColor color)
        {
            return 0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue + color.Alpha;
        }
    }
}