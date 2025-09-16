using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Controls;
using Consolonia.Controls.Brushes;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Consolonia.Core.Text;
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

        // these are magic values for mapping drawing of a line to escape instructions for text decorations around text.
        public const int UnderlineThickness = 10;
        public const int StrikethroughThickness = 11;

        private const int TopLeft = 0;
        private const int TopRight = 1;
        private const int BottomRight = 2;
        private const int BottomLeft = 3;

        private static readonly char[][] EdgeChars =
        [
            // LineStyle=Edge
            ['▕', '▁', '▏', '▔'],
            // LineStyle=EdgeWide
            ['▐', '▄', '▌', '▀']
        ];


        // top left, top right, bottom right, bottom left, 
        private static readonly char[][] EdgeCornerChars =
        [
            // LineStyle=Edge we don't draw chars for edge corners
            [char.MinValue, char.MinValue, char.MinValue, char.MinValue],
            // LineStyle=EdgeWide
            ['▗', '▖', '▘', '▝']
        ];

        private readonly Stack<Rect> _clipStack = new(100);
        private readonly ConsoleWindowImpl _consoleWindowImpl;
        private readonly PixelBuffer _pixelBuffer;
        private readonly Matrix _postTransform = Matrix.Identity;

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly Stack<RenderOptions> _renderOptions = new();
        private Matrix _transform = Matrix.Identity;

        public DrawingContextImpl(ConsoleWindowImpl consoleWindowImpl)
        {
            _consoleWindowImpl = consoleWindowImpl;
            _pixelBuffer = consoleWindowImpl.PixelBuffer;
            _clipStack.Push(_pixelBuffer.Size);
        }

        private Rect CurrentClip => _clipStack.Peek();

        public void Dispose()
        {
        }

        public void Clear(Color color)
        {
            // todo: try to throw an exception here, there will be an exception in logger
            /*if (color != Colors.Transparent)
            {
                ConsoloniaPlatform.RaiseNotSupported(1);
                return;
            }

            _pixelBuffer.Foreach((_, _, _) =>
                new Pixel(PixelBackground.Default));*/
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
            using var skPaint = new SKPaint();
            skPaint.FilterQuality = SKFilterQuality.Medium;
            canvas.DrawBitmap(bmp.Bitmap, new SKRect(0, 0, bitmap.Width, bitmap.Height), skPaint);

            // this is reused by each pixel as we process the bitmap
            Span<SKColor> quadPixelColors = stackalloc SKColor[4];

            int py = (int)Math.Floor(targetRect.TopLeft.Y);
            SKColor[] pixels = bitmap.Pixels;
            int pixelRow = 0;
            for (int y = 0; y < bitmap.Info.Height; y += 2, py++, pixelRow += 2 * bitmap.Width)
            {
                int px = (int)Math.Floor(targetRect.TopLeft.X);
                for (int x = 0; x < bitmap.Info.Width; x += 2, px++)
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
                        new PixelForeground(new SimpleSymbol(quadPixelChar), foreground),
                        new PixelBackground(background));

                    var point = new Point(px, py);
                    if (CurrentClip.ContainsExclusive(point))
                        _pixelBuffer[point] = _pixelBuffer[point].Blend(imagePixel);
                }
            }

            var rectToRefresh = new Rect((int)targetRect.TopLeft.X, (int)targetRect.TopLeft.Y, (int)targetRect.Width,
                (int)targetRect.Height);

            _consoleWindowImpl.DirtyRegions.AddRect(CurrentClip.Intersect(rectToRefresh));
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
                case StreamGeometryImpl streamGeometry:
                {
                    // if we have fills to do.
                    if (streamGeometry.Fills.Count > 0)
                        foreach (Rectangle fill in streamGeometry.Fills)
                            DrawRectangle(brush, pen, new RoundedRect(fill.Rect));

                    // if we have strokes to draw
                    if (streamGeometry.Strokes.Count > 0)
                    {
                        pen ??= new Pen(brush);

                        RectangleLinePosition[] strokePositions = InferStrokePositions(streamGeometry);
                        for (int iStroke = 0; iStroke < streamGeometry.Strokes.Count; iStroke++)
                        {
                            Line stroke = streamGeometry.Strokes[iStroke];
                            RectangleLinePosition strokePosition = strokePositions[iStroke];
                            if (strokePosition == RectangleLinePosition.Left)
                                DrawBoxLineInternal(pen, stroke, RectangleLinePosition.Left);
                            else if (strokePosition == RectangleLinePosition.Right)
                                DrawBoxLineInternal(pen, stroke, RectangleLinePosition.Right);
                            else if (strokePosition == RectangleLinePosition.Top)
                                DrawBoxLineInternal(pen, stroke, RectangleLinePosition.Top);
                            else if (strokePosition == RectangleLinePosition.Bottom)
                                DrawBoxLineInternal(pen, stroke, RectangleLinePosition.Bottom);
                        }
                    }
                }
                    break;
                default:
                    ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawGeometryNotSupported, this, brush,
                        pen, geometry);
                    break;
            }
        }

        public void DrawRectangle(IBrush brush, IPen pen, RoundedRect rect, BoxShadows boxShadows = new())
        {
            if (brush == null && pen == null) return; //this is simple Panel for example

            if (rect.Rect.IsEmpty()) return;

            if (rect.IsRounded)
            {
                ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawingRoundedOrNonUniformRectandle, this,
                    brush, pen, rect, boxShadows);
                // sqaure the rounded corners
                rect = new RoundedRect(rect.Rect, 0.0f, 0.0f, 0.0f, 0.0f);
            }

            if (boxShadows.Count > 0)
                foreach (BoxShadow boxShadow in boxShadows)
                    // BoxShadow none is OK
                    // aka offSetX=0, offSetY=0, color=Transparent
                    if (boxShadow.OffsetX != 0 ||
                        boxShadow.OffsetY != 0 ||
                        boxShadow.Color != Colors.Transparent)
                        ConsoloniaPlatform.RaiseNotSupported(
                            NotSupportedRequestCode.DrawingBoxShadowNotSupported, this, brush, pen, rect, boxShadows);

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
                        sceneBrushContent?.Render(this, Matrix.Identity);
                        return;
                    }
                    case MoveConsoleCaretToPositionBrush moveBrush:
                    {
                        Point head = r.TopLeft.Transform(Transform);
                        if (CurrentClip.ContainsExclusive(head))
                        {
                            Pixel pixel = _pixelBuffer[head];
                            if (pixel.CaretStyle != moveBrush.CaretStyle)
                            {
                                // only be dirty if something changed
                                _consoleWindowImpl.DirtyRegions.AddRect(new Rect(head, new Size(1, 1)));
                                _pixelBuffer[head] =
                                    pixel.Blend(new Pixel(moveBrush.CaretStyle));
                            }
                        }

                        return;
                    }
                }

                FillRectangleWithBrush(brush, r);
            }

            if (pen is null
                or { Thickness: 0 }
                or { Brush: null }
                or { Brush: LineBrush { Brush: null } }) return;
            DrawBoxLineInternal(pen, new Line(r.TopLeft, false, (int)r.Width), RectangleLinePosition.Top);
            DrawBoxLineInternal(pen, new Line(r.BottomLeft, false, (int)r.Width), RectangleLinePosition.Bottom);
            DrawBoxLineInternal(pen, new Line(r.TopLeft, true, (int)r.Height), RectangleLinePosition.Left);
            DrawBoxLineInternal(pen, new Line(r.TopRight, true, (int)r.Height), RectangleLinePosition.Right);
        }


        public void DrawEllipse(IBrush brush, IPen pen, Rect rect)
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawEllipseNotSupported);
        }

        public void DrawGlyphRun(IBrush foreground, IGlyphRunImpl glyphRun)
        {
            if (glyphRun.FontRenderingEmSize.IsNearlyEqual(0)) return;
            if (!glyphRun.FontRenderingEmSize.IsNearlyEqual(1))
            {
                ConsoloniaPlatform.RaiseNotSupported(
                    NotSupportedRequestCode.DrawGlyphRunWithNonDefaultFontRenderingEmSize, this, foreground,
                    glyphRun);
                return;
            }

            if (glyphRun is not GlyphRunImpl glyphRunImpl)
            {
                glyphRunImpl = ConsoloniaPlatform.RaiseNotSupported<GlyphRunImpl>(
                    NotSupportedRequestCode.DrawGlyphRunNotSupported, this, foreground, glyphRun);
                if (glyphRunImpl == null)
                    return;
            }

            var shapedBuffer = (ShapedBuffer)glyphRunImpl.GlyphInfos;
            string text = shapedBuffer.Text.ToString();
            DrawStringInternal(foreground, text, glyphRun.GlyphTypeface);
        }

        public IDrawingContextLayerImpl CreateLayer(PixelSize size)
        {
            return new RenderTarget(_consoleWindowImpl);
        }

        public void PushClip(Rect clip)
        {
            clip = new Rect(clip.Position.Transform(Transform), clip.BottomRight.Transform(Transform));
            _clipStack.Push(CurrentClip.Intersect(clip));
        }

        public void PushClip(RoundedRect clip)
        {
            if (clip.IsRounded)
                ConsoloniaPlatform.RaiseNotSupported(
                    NotSupportedRequestCode.PushClipWithRoundedRectNotSupported, this, clip);

            PushClip(clip.Rect);
        }

        public void PushClip(IPlatformRenderInterfaceRegion region)
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.PushClipRegionNotSupported);

            // we need to keep clipstack aligned even if this is an approximation.
            PushClip(new Rect(region.Bounds.Left,
                region.Bounds.Top,
                region.Bounds.Right - region.Bounds.Left,
                region.Bounds.Bottom - region.Bounds.Top));
        }

        public void PopClip()
        {
            _clipStack.Pop();
        }

        public void PushOpacity(double opacity, Rect? bounds)
        {
            if (opacity.IsNearlyEqual(1)) return;
            ConsoloniaPlatform.RaiseNotSupported(
                NotSupportedRequestCode.PushOpacityNotSupported, this, opacity, bounds);
        }

        public void PopOpacity()
        {
            ConsoloniaPlatform.RaiseNotSupported(
                NotSupportedRequestCode.PushOpacityNotSupported, this);
        }

        public void PushOpacityMask(IBrush mask, Rect bounds)
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.PushOpacityNotSupported);
        }

        public void PopOpacityMask()
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.PushOpacityNotSupported);
        }

        public void PushGeometryClip(IGeometryImpl clip)
        {
            // this is an approximation, we just use the bounds
            PushClip(clip.Bounds);
        }

        public void PopGeometryClip()
        {
            PopClip();
        }

        public void PushRenderOptions(RenderOptions renderOptions)
        {
            _renderOptions.Push(renderOptions);
        }

        public void PopRenderOptions()
        {
            _renderOptions.Pop();
        }

        public object GetFeature(Type t)
        {
            throw new NotImplementedException();
        }

        public Matrix Transform
        {
            get => _transform;
            set => _transform = value * _postTransform;
        }

        public void DrawRegion(IBrush brush, IPen pen, IPlatformRenderInterfaceRegion region)
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawRegionNotSupported);
        }

        public void PushLayer(Rect bounds)
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.PushLayerNotSupported);
        }

        public void PopLayer()
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.PushLayerNotSupported);
        }

        private static RectangleLinePosition[] InferStrokePositions(StreamGeometryImpl streamGeometry)
        {
            // infer rectangle hints by using focolpoint
            double focalPointX = streamGeometry.Strokes.Average(stroke =>
                stroke.PStart.X + Math.Abs(stroke.PStart.X - stroke.PEnd.X) / 2);
            double focalPointY = streamGeometry.Strokes.Average(stroke =>
                stroke.PStart.Y + Math.Abs(stroke.PStart.Y - stroke.PEnd.Y) / 2);
            var focalPoint = new Point(focalPointX, focalPointY);
            var strokePositions = new RectangleLinePosition[streamGeometry.Strokes.Count];

            for (int i = 0; i < streamGeometry.Strokes.Count; i++)
            {
                Line stroke = streamGeometry.Strokes[i];
                if (stroke.Bounds is { Width: 0, Height: 0 })
                {
                    // ignore zero length strokes
                    strokePositions[i] = RectangleLinePosition.Unknown;
                    continue;
                }

                if (stroke.Vertical)
                {
                    if (stroke.PStart.X <= focalPoint.X)
                        strokePositions[i] = RectangleLinePosition.Left;
                    else if (stroke.PStart.X >= focalPoint.X)
                        strokePositions[i] = RectangleLinePosition.Right;
                }
                else
                {
                    if (stroke.PStart.Y <= focalPoint.Y)
                        strokePositions[i] = RectangleLinePosition.Top;
                    else if (stroke.PStart.Y >= focalPoint.Y)
                        strokePositions[i] = RectangleLinePosition.Bottom;
                }
            }

            return strokePositions;
        }

        /// <summary>
        ///     Draw a straight horizontal line or vertical line
        /// </summary>
        /// <param name="pen">pen</param>
        /// <param name="line">line</param>
        private void DrawLineInternal(IPen pen, Line line)
        {
            if (pen.Thickness.IsNearlyEqual(0)) return;

            if (pen.Thickness.IsNearlyEqual(UnderlineThickness) || pen.Thickness.IsNearlyEqual(StrikethroughThickness))
            {
                if (line.Vertical)
                    throw new NotSupportedException(
                        "Vertical strikethrough or underline text decorations is not supported.");

                // horizontal lines with thickness larger than one are text decorations
                ApplyTextDecorationLineInternal(pen, line);
                return;
            }

            if (pen.Brush is MoveConsoleCaretToPositionBrush moveBrush)
            {
                //todo low: same code is above also
                Point head = line.PStart.Transform(Transform);
                if (CurrentClip.ContainsExclusive(head))
                {
                    _pixelBuffer[head] = _pixelBuffer[head].Blend(new Pixel(moveBrush.CaretStyle));
                    _consoleWindowImpl.DirtyRegions.AddRect(CurrentClip.Intersect(new Rect(head, new Size(1, 1))));
                }

                return;
            }

            DrawBoxLineInternal(pen, line, RectangleLinePosition.Unknown);
        }

        private void ApplyTextDecorationLineInternal(IPen pen, Line line)
        {
            line = TransformLineInternal(line);

            var rectToRefresh = new Rect((int)line.PStart.X, (int)line.PStart.Y, line.Length, 1);
            Rect intersectRect = CurrentClip.Intersect(rectToRefresh);
            if (intersectRect.IsEmpty())
                return;

            Point head = intersectRect.TopLeft;

            TextDecorationLocation textDecoration = pen.Thickness switch
            {
                UnderlineThickness => TextDecorationLocation.Underline,
                StrikethroughThickness => TextDecorationLocation.Strikethrough,
                _ => throw new ArgumentOutOfRangeException($"Unsupported thickness {pen.Thickness}")
            };

            for (int x = (int)intersectRect.Left; x < intersectRect.Right; x++)
            {
                Pixel oldPixel = _pixelBuffer[head];
                var newPixelForeground = new PixelForeground(oldPixel.Foreground.Symbol,
                    oldPixel.Foreground.Color,
                    oldPixel.Foreground.Weight,
                    oldPixel.Foreground.Style,
                    textDecoration);
                _pixelBuffer[head] = oldPixel.Blend(new Pixel(newPixelForeground));

                head = head.WithX(head.X + 1);
            }

            _consoleWindowImpl.DirtyRegions.AddRect(intersectRect);
        }

        private void FillRectangleWithBrush(IBrush brush, Rect r)
        {
            if (brush is ISolidColorBrush { Color.A: 0 })
                return;

            // fill rectangle with brush
            Rect sourceRect = r.TransformToAABB(Transform);
            Rect targetRect = CurrentClip.Intersect(sourceRect);

            if (targetRect.IsEmpty())
                return;

            // Clamp to valid range to prevent out-of-bounds errors
            ushort gradiantWidth = (ushort)Math.Max(1, Math.Ceiling(sourceRect.Width));
            ushort gradiantHeight = (ushort)Math.Max(1, Math.Ceiling(sourceRect.Height));
            ushort brushY = (ushort)(targetRect.Top - sourceRect.Top);
            for (ushort y = (ushort)targetRect.Top; y < targetRect.Bottom; y++, brushY++)
            {
                ushort brushX = (ushort)(targetRect.Left - sourceRect.Left);
                for (ushort x = (ushort)targetRect.Left; x < targetRect.Right; x++, brushX++)
                {
                    Color backgroundColor = brush.FromPosition(brushX, brushY, gradiantWidth, gradiantHeight);

                    switch (brush)
                    {
                        case ShadeBrush:
                            _pixelBuffer[x, y] = _pixelBuffer[x, y].Shade();
                            break;
                        case BrightenBrush:
                            _pixelBuffer[x, y] = _pixelBuffer[x, y].Brighten();
                            break;
                        case InvertBrush:
                            _pixelBuffer[x, y] = _pixelBuffer[x, y].Invert();
                            break;
                        default:
                            _pixelBuffer[x, y] =
                                _pixelBuffer[x, y].Blend(new Pixel(new PixelBackground(backgroundColor)));
                            break;
                    }
                }
            }

            _consoleWindowImpl.DirtyRegions.AddRect(targetRect);
        }

        /// <summary>
        ///     Draw a rectangle line with corners
        /// </summary>
        /// <param name="pen">pen</param>
        /// <param name="line">line</param>
        /// <param name="linePosition">The relative rectangle line position</param>
        private void DrawBoxLineInternal(IPen pen, Line line, RectangleLinePosition linePosition)
        {
            if (pen.Thickness == 0) return;

            line = TransformLineInternal(line);

            Point head = line.PStart;

            Color? extractColorCheckPlatformSupported =
                ExtractColorOrNullWithPlatformCheck(pen, out LineStyles lineStyles);
            if (extractColorCheckPlatformSupported == null)
                return;

            var color = (Color)extractColorCheckPlatformSupported;

            LineStyle lineStyle = linePosition switch
            {
                RectangleLinePosition.Top => lineStyles.Top,
                RectangleLinePosition.Right => lineStyles.Right,
                RectangleLinePosition.Bottom => lineStyles.Bottom,
                RectangleLinePosition.Left => lineStyles.Left,
                _ => LineStyle.SingleLine
            };

            if (lineStyle is LineStyle.Edge or LineStyle.EdgeWide)
            {
                DrawEdgeLine(line, linePosition, lineStyle, color);
            }
            else
            {
                byte pattern = line.Vertical ? VerticalStartPattern : HorizontalStartPattern;
                DrawBoxPixelAndMoveHead(ref head, line, lineStyle, pattern, color, 1); //beginning

                pattern = line.Vertical ? VerticalLinePattern : HorizontalLinePattern;
                DrawBoxPixelAndMoveHead(ref head, line, lineStyle, pattern, color, line.Length - 1); //line

                pattern = line.Vertical ? VerticalEndPattern : HorizontalEndPattern;
                DrawBoxPixelAndMoveHead(ref head, line, lineStyle, pattern, color, 1); //ending 
            }
        }

        private void DrawEdgeLine(Line line, RectangleLinePosition linePosition, LineStyle lineStyle, Color color)
        {
            if (line.Length == 0)
                return;
            ISymbol startSymbol;
            ISymbol middleSymbol;
            ISymbol endSymbol;
            int iStyle = lineStyle == LineStyle.Edge ? 0 : 1;

            switch (linePosition)
            {
                case RectangleLinePosition.Left:
                    startSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][TopLeft]);
                    middleSymbol = new SimpleSymbol(EdgeChars[iStyle][(int)RectangleLinePosition.Left]);
                    endSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][BottomLeft]);
                    break;
                case RectangleLinePosition.Top:
                    startSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][TopLeft]);
                    middleSymbol = new SimpleSymbol(EdgeChars[iStyle][(int)RectangleLinePosition.Top]);
                    endSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][TopRight]);
                    break;
                case RectangleLinePosition.Right:
                    startSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][TopRight]);
                    middleSymbol = new SimpleSymbol(EdgeChars[iStyle][(int)RectangleLinePosition.Right]);
                    endSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][BottomRight]);
                    break;
                case RectangleLinePosition.Bottom:
                    startSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][BottomLeft]);
                    middleSymbol = new SimpleSymbol(EdgeChars[iStyle][(int)RectangleLinePosition.Bottom]);
                    endSymbol = new SimpleSymbol(EdgeCornerChars[iStyle][BottomRight]);
                    break;
                default:
                    throw new NotImplementedException("This shouldn't happen");
            }

            Point head = line.PStart;

            int length = line.Length;
            if (startSymbol.Text != "\0")
                DrawLineSymbolAndMoveHead(ref head, line.Vertical, in startSymbol, color, 1);
            else
                head += line.Vertical ? new Vector(0, 1) : new Vector(1, 0);

            DrawLineSymbolAndMoveHead(ref head, line.Vertical, in middleSymbol, color, length - 1);

            if (endSymbol.Text != "\0")
                DrawLineSymbolAndMoveHead(ref head, line.Vertical, in endSymbol, color, 1);
        }

        /// <summary>
        ///     Transform line coordinates
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private Line TransformLineInternal(Line line)
        {
            if (!Transform.NoRotation())
                return ConsoloniaPlatform.RaiseNotSupported<Line>(
                    NotSupportedRequestCode.TransformLineWithRotationNotSupported, this, line);

            line = (Line)line.WithTransform(Transform);
            return line;
        }

        /// <summary>
        ///     Extract color from pen brush
        /// </summary>
        /// <param name="pen"></param>
        /// <param name="lineStyles"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private static Color? ExtractColorOrNullWithPlatformCheck(IPen pen, out LineStyles lineStyles)
        {
            lineStyles = null;
            if (pen is not
                {
                    Brush: LineBrush or ISolidColorBrush
                    // Thickness: 1,
                    // DashStyle: null or { Dashes.Count: 0 },
                    //LineCap: PenLineCap.Flat,
                    //LineJoin: PenLineJoin.Miter
                })
            {
                (Color? color, lineStyles) = ConsoloniaPlatform.RaiseNotSupported<(Color?, LineStyles)>(
                    NotSupportedRequestCode.ExtractColorFromPenNotSupported, null, pen);
                return color;
            }

            IBrush brush = pen.Brush;

            if (brush is LineBrush lineBrush)
            {
                lineStyles = lineBrush.LineStyle;
                brush = lineBrush.Brush;
            }
            else
            {
                lineStyles = new LineStyles();
            }

            return ((ISolidColorBrush)brush).Color;
        }

        /// <summary>
        ///     Draw pixels for a line with line style and a pattern
        /// </summary>
        /// <param name="head">the current caret position</param>
        /// <param name="line">line to render</param>
        /// <param name="lineStyle">line style</param>
        /// <param name="pattern">pattern of character to use</param>
        /// <param name="color">color for char</param>
        /// <param name="count">number of chars</param>
        private void DrawBoxPixelAndMoveHead(ref Point head, Line line, LineStyle lineStyle, byte pattern, Color color,
            int count)
        {
            Rect rectToRefresh = line.Vertical
                ? new Rect((int)head.X, (int)head.Y, 1, count)
                : new Rect((int)head.X, (int)head.Y, count, 1);
            Rect intersect = CurrentClip.Intersect(rectToRefresh);
            if (intersect.IsEmpty())
                return;

            ushort start = line.Vertical ? (ushort)intersect.Top : (ushort)intersect.Left;
            ushort end = line.Vertical ? (ushort)intersect.Bottom : (ushort)intersect.Right;
            // align head with the first intersected point
            head = line.Vertical ? head.WithY(start) : head.WithX(start);
            for (ushort i = start; i < end; i++)
            {
                _pixelBuffer[head] =
                    _pixelBuffer[head].Blend(new Pixel(DrawingBoxSymbol.UpRightDownLeftFromPattern(pattern, lineStyle),
                        color));

                head = line.Vertical
                    ? head.WithY(head.Y + 1)
                    : head.WithX(head.X + 1);
            }

            _consoleWindowImpl.DirtyRegions.AddRect(intersect);
        }

        private void DrawLineSymbolAndMoveHead(ref Point head, bool isVertical, in ISymbol symbol, Color color,
            int count)
        {
            Rect rectToRefresh = isVertical
                ? new Rect((int)head.X, (int)head.Y, 1, count)
                : new Rect((int)head.X, (int)head.Y, count, 1);
            Rect intersect = CurrentClip.Intersect(rectToRefresh);
            if (intersect.IsEmpty())
                return;

            ushort start = isVertical ? (ushort)intersect.Top : (ushort)intersect.Left;
            ushort end = isVertical ? (ushort)intersect.Bottom : (ushort)intersect.Right;
            // align head with the first intersected point
            head = isVertical ? head.WithY(start) : head.WithX(start);
            var newPixel = new Pixel(symbol, color);
            for (int i = start; i < end; i++)
            {
                _pixelBuffer[head] = _pixelBuffer[head].Blend(newPixel);

                head = isVertical
                    ? head.WithY(head.Y + 1)
                    : head.WithX(head.X + 1);
            }

            _consoleWindowImpl.DirtyRegions.AddRect(intersect);
        }

        private void DrawStringInternal(IBrush foreground, string text, IGlyphTypeface typeface, Point origin = new())
        {
            if (foreground is not ISolidColorBrush solidColorBrush)
            {
                solidColorBrush = ConsoloniaPlatform.RaiseNotSupported<ISolidColorBrush>(
                    NotSupportedRequestCode.DrawStringWithNonSolidColorBrush, this, foreground, text, typeface, origin);

                if (solidColorBrush == null)
                    return;
            }

            // if (!Transform.IsTranslateOnly()) ConsoloniaPlatform.RaiseNotSupported(15); //todo: what to do if a rotation?

            Point position = origin.Transform(Transform);
            double lineStartX = position.X;

            // Each glyph maps to a pixel as a starting point.
            // Emoji's and Ligatures are complex strings, so they start at a point and then overlap following pixels
            // the x and y are adjusted accordingly.
            foreach (string glyph in text.GetGlyphs(_consoleWindowImpl.Console.SupportsComplexEmoji))
            {
                Color foregroundColor = solidColorBrush.Color;

                switch (glyph)
                {
                    case "\t":
                        position = position.WithX(position.X + glyph.MeasureText());
                        break;
                    case "\r":
                    case "\f":
                    case "\n":
                        position = new Point(lineStartX, position.Y + 1);
                        break;
                    default:
                    {
                        var symbol = new SimpleSymbol(glyph);
                        // if we are attempting to draw a wide glyph we need to make sure that the clipping point
                        // is for the last physical char. Aka a double char should be clipped if it's second rendered 
                        // char would break the boundary of the clip.
                        // var clippingPoint = new Point(characterPoint.X + symbol.Width - 1, characterPoint.Y);
                        var newPixel = new Pixel(symbol, foregroundColor, typeface.Style, typeface.Weight);
                        if (CurrentClip.ContainsExclusive(position))
                        {
                            Pixel oldPixel = _pixelBuffer[position];
                            if (oldPixel.Width == 0)
                            {
                                // if the oldPixel was empty, we need to set the previous pixel to space
                                Point target = position.WithX(position.X - 1);
                                if (target.X >= 0)
                                    _pixelBuffer[target] = new Pixel(PixelForeground.Space,
                                        _pixelBuffer[target].Background);
                            }
                            else if (oldPixel.Width > 1)
                            {
                                // if oldPixel was wide we need to reset overlapped symbols from empty to space
                                for (ushort i = 1; i < oldPixel.Width; i++)
                                {
                                    Point target = position.WithX(position.X + i);
                                    if (target.X < _pixelBuffer.Size.Width)
                                        _pixelBuffer[target] = new Pixel(PixelForeground.Space,
                                            _pixelBuffer[target].Background);
                                }
                            }

                            // if the pixel was a wide character, we need to set the overlapped pixels to empty pixels.
                            if (newPixel.Width > 1)
                                for (int i = 1; i < symbol.Width; i++)
                                {
                                    Point target = position.WithX(position.X + i);
                                    if (target.X < _pixelBuffer.Size.Width)
                                        _pixelBuffer[target] = new Pixel(PixelForeground.Empty,
                                            _pixelBuffer[target].Background);
                                }

                            _pixelBuffer[position] = oldPixel.Blend(newPixel);
                        }

                        position = position.WithX(position.X + symbol.Width);
                    }
                        break;
                }
            }

            // Width/height are exclusive, so add 1 to include the last column/row
            var rectToRefresh = new Rect((int)position.X, (int)position.Y, position.X + 1,
                position.Y + 1);
            _consoleWindowImpl.DirtyRegions.AddRect(CurrentClip.Intersect(rectToRefresh));
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