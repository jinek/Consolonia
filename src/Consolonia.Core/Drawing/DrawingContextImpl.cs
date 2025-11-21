//DUPFINDER_ignore
//todo: this file is under refactoring. Restore the duplication finder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Consolonia.Controls.Brushes;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Consolonia.Core.Text;
using Consolonia.Core.Text.Fonts;

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

        private readonly Stack<PixelRect> _clipStack = new(100);
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

        private PixelRect CurrentClip => _clipStack.Peek();

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
            if (source is DummyBitmap)
                return;

            var targetRect = new Rect(Transform.Transform(destRect.TopLeft),
                    Transform.Transform(destRect.BottomRight))
                .ToPixelRect();

            var renderInterface = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>();

            // Resize source to be target rect * 2 so we can map to quad pixels
            var targetSize = new PixelSize(targetRect.Width * 2, targetRect.Height * 2);
            using IBitmapImpl resizedBitmap =
                renderInterface.ResizeBitmap(source, targetSize, BitmapInterpolationMode.MediumQuality);

            var readableBitmap = (IReadableBitmapImpl)resizedBitmap;

            using ILockedFramebuffer frameBuffer = readableBitmap.Lock();

            int stride = frameBuffer.RowBytes;
            int bytesPerPixel = frameBuffer.Format.BitsPerPixel / 8;
            unsafe
            {
                ReadOnlySpan<byte> pixelBytes = MemoryMarshal.CreateReadOnlySpan(
                    ref Unsafe.AsRef<byte>((void*)frameBuffer.Address), stride * frameBuffer.Size.Height);
                ReadOnlySpan<BgraColor> pixels = MemoryMarshal.Cast<byte, BgraColor>(pixelBytes);


                int py = targetRect.TopLeft.Y;

                for (int y = 0; y < targetSize.Height; y += 2, py++)
                {
                    int px = targetRect.TopLeft.X;
                    for (int x = 0; x < targetSize.Width; x += 2, px++)
                    {
                        var point = new PixelPoint(px, py);
                        if (CurrentClip.ContainsExclusive(point))
                        {
                            // get the quad pixel from the bitmap as a quad of 4 BgraColor values
                            Span<BgraColor> quadPixelColors =
                            [
                                GetPixelColor(pixels, x, y, stride, bytesPerPixel),
                                GetPixelColor(pixels, x + 1, y, stride, bytesPerPixel),
                                GetPixelColor(pixels, x, y + 1, stride, bytesPerPixel),
                                GetPixelColor(pixels, x + 1, y + 1, stride, bytesPerPixel)
                            ];

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
            }

            _consoleWindowImpl.DirtyRegions.AddRect(targetRect);
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
                    Rect rect = myRectangle.Rect;
                    DrawRectangle(brush, pen, new RoundedRect(rect));
                    break;
                case Line myLine:
                    DrawLineInternal(pen, myLine);
                    break;
                case StreamGeometryImpl streamGeometry:
                {
                    // if we have fills to do and a brush with opacity
                    if (brush != null &&
                        brush.Opacity > 0 &&
                        streamGeometry.Fills.Count > 0)
                        foreach (Rectangle fill in streamGeometry.Fills)
                            // Investigate: Does the pen apply to rectangle or not?
                            DrawRectangle(brush, null, new RoundedRect(fill.Rect));

                    // if we have strokes to draw, and a valid pen 
                    if (pen != null &&
                        pen.Thickness > 0 &&
                        pen.Brush != null &&
                        pen.Brush.Opacity > 0 &&
                        streamGeometry.Strokes.Count > 0)
                    {
                        RectangleLinePosition[] strokePositions = InferStrokePositions(streamGeometry);
                        for (int iStroke = 0; iStroke < streamGeometry.Strokes.Count; iStroke++)
                        {
                            Line stroke = streamGeometry.Strokes[iStroke];
                            RectangleLinePosition strokePosition = strokePositions[iStroke];
                            if (strokePosition == RectangleLinePosition.Left)
                                DrawLineInternal(pen, stroke, RectangleLinePosition.Left);
                            else if (strokePosition == RectangleLinePosition.Right)
                                DrawLineInternal(pen, stroke, RectangleLinePosition.Right);
                            else if (strokePosition == RectangleLinePosition.Top)
                                DrawLineInternal(pen, stroke, RectangleLinePosition.Top);
                            else if (strokePosition == RectangleLinePosition.Bottom)
                                DrawLineInternal(pen, stroke, RectangleLinePosition.Bottom);
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

        /// <summary>
        ///     Draw a rectangle, rounded rectangles are not supported and will be drawn as normal rectangles.
        /// </summary>
        /// <remarks>
        ///     NOTE: Rect in avalonia has interesting semantics
        ///     Left and Top are INCLUSIVE
        ///     Right and Bottom are EXCLUSIVE.
        ///     So width=3 and height=3 means pixels at 0,1,2 even though Right = 4 and Bottom = 4
        ///     Bottom and Right cells should NOT be drawn into.
        /// </remarks>
        /// <param name="brush"></param>
        /// <param name="pen"></param>
        /// <param name="roundedRect"></param>
        /// <param name="boxShadows"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void DrawRectangle(IBrush brush, IPen pen, RoundedRect roundedRect, BoxShadows boxShadows = new())
        {
            if (roundedRect.Rect.IsEmpty()) return;

            if (roundedRect.IsRounded)
                ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawingRoundedOrNonUniformRectandle, this,
                    brush, pen, roundedRect, boxShadows);

            Rect rect = roundedRect.Rect;
            if (pen != null && brush != null)
            {
                // This is one of those places where Avalonia/Consolonia don't align well due to character nature of consolonia.
                //
                // in this case the Rectangle geometry passes us a rect that is 1 pixel smaller than the pen thickness
                // based on the brush we need to adjust
                // * single/doubleline brushes we need to expand the fill to be 1 pixel larger on each side
                // * Edge brushes we need to shrink the fill to be 1 pixel smaller on each side
                if (pen.Brush is LineBrush lineBrush && lineBrush.HasEdgeLineStyle())
                    // shrink fill so that edge pen can be drawn around it.
                    DrawRectangleInternal(brush, null,
                        new Rect(rect.Position.X + 1, rect.Position.Y + 1, rect.Width - 1, rect.Height - 1));
                else
                    // increase fill so that it includes the border pen.
                    DrawRectangleInternal(brush, null,
                        new Rect(rect.Position, new Size(rect.Size.Width + 1, rect.Size.Height + 1)));
                DrawRectangleInternal(null, pen, rect, boxShadows);
            }
            else
            {
                // just draw the brush or pen 
                DrawRectangleInternal(brush, pen, rect, boxShadows);
            }
        }


        public void DrawEllipse(IBrush brush, IPen pen, Rect rect)
        {
            ConsoloniaPlatform.RaiseNotSupported(NotSupportedRequestCode.DrawEllipseNotSupported);
        }

        public void DrawGlyphRun(IBrush foreground, IGlyphRunImpl glyphRun)
        {
            if (glyphRun.FontRenderingEmSize.IsNearlyEqual(0)) return;

            if (glyphRun is not GlyphRunImpl glyphRunImpl)
            {
                glyphRunImpl = ConsoloniaPlatform.RaiseNotSupported<GlyphRunImpl>(
                    NotSupportedRequestCode.DrawGlyphRunNotSupported, this, foreground, glyphRun);
                if (glyphRunImpl == null)
                    return;
            }

            if (foreground is not ISolidColorBrush solidColorBrush)
            {
                solidColorBrush = ConsoloniaPlatform.RaiseNotSupported<ISolidColorBrush>(
                    NotSupportedRequestCode.DrawStringWithNonSolidColorBrush, this, foreground);

                if (solidColorBrush == null)
                    return;
            }

            var glyphTypefaceRender = (IGlyphRunRender)glyphRun.GlyphTypeface;
            Color foregroundColor = solidColorBrush.Color;
            var startPosition = new Point().Transform(Transform).ToPixelPoint();
            glyphTypefaceRender.DrawGlyphRun(this, startPosition, glyphRunImpl, foregroundColor,
                out PixelRect rectToRefresh);

            _consoleWindowImpl.DirtyRegions.AddRect(rectToRefresh);
        }

        public IDrawingContextLayerImpl CreateLayer(PixelSize size)
        {
            return new RenderTarget(_consoleWindowImpl);
        }

        public void PushClip(Rect clip)
        {
            clip = new Rect(clip.Position.Transform(Transform), clip.BottomRight.Transform(Transform));
            _clipStack.Push(CurrentClip.Intersect(clip.ToPixelRect()));
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

        private void DrawRectangleInternal(IBrush brush, IPen pen, Rect rect, BoxShadows boxShadows = new())
        {
            if (brush == null && pen == null) return; //this is simple Panel for example

            Rect rectangleRect = rect.TransformToAABB(Transform);
            if (boxShadows.Count > 0)
                foreach (BoxShadow boxShadow in boxShadows)
                    // BoxShadow none is OK
                    // aka offSetX=0, offSetY=0, color=Transparent
                    if (boxShadow.OffsetX != 0 ||
                        boxShadow.OffsetY != 0 ||
                        boxShadow.Color != Colors.Transparent)
                        ConsoloniaPlatform.RaiseNotSupported(
                            NotSupportedRequestCode.DrawingBoxShadowNotSupported, this, brush, pen, rect, boxShadows);

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
                        var head = rectangleRect.TopLeft.ToPixelPoint();
                        if (CurrentClip.ContainsExclusive(head))
                        {
                            Pixel pixel = _pixelBuffer[head];
                            if (pixel.CaretStyle != moveBrush.CaretStyle)
                            {
                                // only be dirty if something changed
                                _consoleWindowImpl.DirtyRegions.AddRect(new PixelRect(head, new PixelSize(1, 1)));
                                _pixelBuffer[head] =
                                    pixel.Blend(new Pixel(moveBrush.CaretStyle));
                            }
                        }

                        return;
                    }
                }

                FillRectangleWithBrush(brush, rectangleRect.ToPixelRect());
            }

            if (pen is null
                or { Thickness: 0 }
                or { Brush: null }
                or { Brush: LineBrush { Brush: null } }) return;

            // NOTE: Line takes in untransformed Point, not PixelPoint and will be transformed inside DrawLineInternal
            DrawLineInternal(pen, new Line(rect.TopLeft, /*vertical: */ false, (int)rect.Width),
                RectangleLinePosition.Top);
            DrawLineInternal(pen, new Line(rect.BottomLeft, /*vertical: */ false, (int)rect.Width),
                RectangleLinePosition.Bottom);
            DrawLineInternal(pen, new Line(rect.TopLeft, /*vertical: */ true, (int)rect.Height),
                RectangleLinePosition.Left);
            DrawLineInternal(pen, new Line(rect.TopRight, /*vertical: */ true, (int)rect.Height),
                RectangleLinePosition.Right);
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
                var head = line.PStart.Transform(Transform).ToPixelPoint();
                if (CurrentClip.ContainsExclusive(head))
                {
                    _pixelBuffer[head] = _pixelBuffer[head].Blend(new Pixel(moveBrush.CaretStyle));
                    _consoleWindowImpl.DirtyRegions.AddRect(
                        CurrentClip.Intersect(new PixelRect(head, new PixelSize(1, 1))));
                }

                return;
            }

            DrawLineInternal(pen, line, RectangleLinePosition.Unknown);
        }

        private void ApplyTextDecorationLineInternal(IPen pen, Line line)
        {
            line = TransformLineInternal(line);

            var rectToRefresh = new PixelRect(line.PStart.ToPixelPoint(), new PixelSize(line.Length, 1));
            PixelRect intersectRect = CurrentClip.Intersect(rectToRefresh);
            if (intersectRect.IsEmpty())
                return;

            PixelPoint head = intersectRect.TopLeft;

            TextDecorationLocation textDecoration = pen.Thickness switch
            {
                UnderlineThickness => TextDecorationLocation.Underline,
                StrikethroughThickness => TextDecorationLocation.Strikethrough,
                _ => throw new ArgumentOutOfRangeException($"Unsupported thickness {pen.Thickness}")
            };

            for (int x = intersectRect.X; x < intersectRect.Right; x++)
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

        private void FillRectangleWithBrush(IBrush brush, PixelRect pixelRect)
        {
            Pixel solidPixel = default;
            var solidColorBrush = brush as ISolidColorBrush;
            if (solidColorBrush != null)
            {
                Color solidColor = solidColorBrush.Color;
                if (solidColor is { A: 0 }) return;

                solidPixel = new Pixel(new PixelBackground(solidColor));
            }

            // fill rectangle with brush
            PixelRect targetRect = CurrentClip.Intersect(pixelRect);

            if (targetRect.IsEmpty())
                return;

            // Clamp to valid range to prevent out-of-bounds errors
            ushort gradiantWidth = (ushort)Math.Max(1, pixelRect.Width);
            ushort gradiantHeight = (ushort)Math.Max(1, pixelRect.Height);
            ushort brushY = (ushort)(targetRect.Y - pixelRect.Y);

            switch (brush)
            {
                case ShadeBrush:
                    for (ushort y = (ushort)targetRect.Y; y < targetRect.Bottom; y++, brushY++)
                    for (ushort x = (ushort)targetRect.X; x < targetRect.Right; x++)
                        _pixelBuffer[x, y] = _pixelBuffer[x, y].Shade();
                    break;
                case BrightenBrush:
                    for (ushort y = (ushort)targetRect.Y; y < targetRect.Bottom; y++, brushY++)
                    for (ushort x = (ushort)targetRect.X; x < targetRect.Right; x++)
                        _pixelBuffer[x, y] = _pixelBuffer[x, y].Brighten();
                    break;
                case InvertBrush:
                    for (ushort y = (ushort)targetRect.Y; y < targetRect.Bottom; y++, brushY++)
                    for (ushort x = (ushort)targetRect.X; x < targetRect.Right; x++)
                        _pixelBuffer[x, y] = _pixelBuffer[x, y].Invert();
                    break;
                default:
                    for (ushort y = (ushort)targetRect.Y; y < (ushort)targetRect.Bottom; y++, brushY++)
                    {
                        ushort brushX = (ushort)(targetRect.X - pixelRect.X);
                        for (ushort x = (ushort)targetRect.X; x < targetRect.Right; x++, brushX++)
                        {
                            Pixel pixelAbove;
                            if (solidColorBrush == null)
                            {
                                Color backgroundColor =
                                    brush.FromPosition(brushX, brushY, gradiantWidth, gradiantHeight);
                                pixelAbove = new Pixel(new PixelBackground(backgroundColor));
                            }
                            else
                            {
                                pixelAbove = solidPixel;
                            }

                            _pixelBuffer[x, y] = _pixelBuffer[x, y].Blend(pixelAbove);
                        }
                    }

                    break;
            }

            _consoleWindowImpl.DirtyRegions.AddRect(targetRect);
        }

        /// <summary>
        ///     Draw a rectangle line with corners
        /// </summary>
        /// <param name="pen">pen</param>
        /// <param name="line">line</param>
        /// <param name="linePosition">The relative rectangle line position</param>
        private void DrawLineInternal(IPen pen, Line line, RectangleLinePosition linePosition)
        {
            if (pen.Thickness == 0) return;

            line = TransformLineInternal(line);

            Color? foregroundColor =
                ExtractColorOrNullWithPlatformCheck(pen, out LineStyles lineStyles);
            if (foregroundColor == null)
                return;

            LineStyle lineStyle = linePosition switch
            {
                RectangleLinePosition.Top => lineStyles.Top,
                RectangleLinePosition.Right => lineStyles.Right,
                RectangleLinePosition.Bottom => lineStyles.Bottom,
                RectangleLinePosition.Left => lineStyles.Left,
                _ => LineStyle.SingleLine
            };

            if (lineStyle is LineStyle.Edge or LineStyle.EdgeWide)
                DrawEdgeLine(line, linePosition, lineStyle, (Color)foregroundColor);
            else
                DrawBoxLine(line, lineStyle, (Color)foregroundColor);
        }

        private void DrawBoxLine(Line line, LineStyle lineStyle, Color color)
        {
            if (line.Length == 0)
                return;

            var head = line.PStart.ToPixelPoint();

            byte pattern = line.Vertical ? VerticalStartPattern : HorizontalStartPattern;
            var symbol = new Symbol(GetBoxPatternFromLineStyle(pattern, lineStyle));
            DrawLineSymbolAndMoveHead(ref head, line.Vertical, in symbol, color, 1); //beginning

            pattern = line.Vertical ? VerticalLinePattern : HorizontalLinePattern;
            symbol = new Symbol(GetBoxPatternFromLineStyle(pattern, lineStyle));
            DrawLineSymbolAndMoveHead(ref head, line.Vertical, in symbol, color, line.Length - 1); //line

            pattern = line.Vertical ? VerticalEndPattern : HorizontalEndPattern;
            symbol = new Symbol(GetBoxPatternFromLineStyle(pattern, lineStyle));
            DrawLineSymbolAndMoveHead(ref head, line.Vertical, in symbol, color, 1); //ending
        }

        private void DrawEdgeLine(Line line, RectangleLinePosition linePosition, LineStyle lineStyle, Color color)
        {
            if (line.Length == 0)
                return;
            Symbol startSymbol;
            Symbol middleSymbol;
            Symbol endSymbol;
            int iStyle = lineStyle == LineStyle.Edge ? 0 : 1;

            switch (linePosition)
            {
                case RectangleLinePosition.Left:
                    startSymbol = new Symbol(EdgeCornerChars[iStyle][TopLeft]);
                    middleSymbol = new Symbol(EdgeChars[iStyle][(int)RectangleLinePosition.Left]);
                    endSymbol = new Symbol(EdgeCornerChars[iStyle][BottomLeft]);
                    break;
                case RectangleLinePosition.Top:
                    startSymbol = new Symbol(EdgeCornerChars[iStyle][TopLeft]);
                    middleSymbol = new Symbol(EdgeChars[iStyle][(int)RectangleLinePosition.Top]);
                    endSymbol = new Symbol(EdgeCornerChars[iStyle][TopRight]);
                    break;
                case RectangleLinePosition.Right:
                    startSymbol = new Symbol(EdgeCornerChars[iStyle][TopRight]);
                    middleSymbol = new Symbol(EdgeChars[iStyle][(int)RectangleLinePosition.Right]);
                    endSymbol = new Symbol(EdgeCornerChars[iStyle][BottomRight]);
                    break;
                case RectangleLinePosition.Bottom:
                    startSymbol = new Symbol(EdgeCornerChars[iStyle][BottomLeft]);
                    middleSymbol = new Symbol(EdgeChars[iStyle][(int)RectangleLinePosition.Bottom]);
                    endSymbol = new Symbol(EdgeCornerChars[iStyle][BottomRight]);
                    break;
                default:
                    throw new NotImplementedException("This shouldn't happen");
            }

            var head = line.PStart.ToPixelPoint();

            int length = line.Length;
            DrawLineSymbolAndMoveHead(ref head, line.Vertical, in startSymbol, color, 1);
            DrawLineSymbolAndMoveHead(ref head, line.Vertical, in middleSymbol, color, length - 1);
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


        private static byte GetBoxPatternFromLineStyle(byte pattern, LineStyle lineStyle)
        {
            switch (lineStyle)
            {
                case LineStyle.SingleLine:
                    return pattern;
                case LineStyle.DoubleLine:
                    return (byte)((pattern << 4) | pattern);
                case LineStyle.Bold:
                    return BoxPattern.BoldPattern;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lineStyle), lineStyle, null);
            }
        }

        public void DrawPixel(Pixel pixel, PixelPoint position)
        {
            if (CurrentClip.ContainsExclusive(position)) _pixelBuffer[position] = _pixelBuffer[position].Blend(pixel);
        }

        /// <summary>
        ///     Draw a line symbol and move the head position.
        /// </summary>
        /// <remarks>
        ///     This moves the head position by count in the appropriate direction.
        /// </remarks>
        /// <param name="head">starting point IN PIXELPOINT COORDINATES THAT CAN BE NEGATIVE</param>
        /// <param name="isVertical">is vertical or horizontal head advancement</param>
        /// <param name="symbol">symbol to draw with</param>
        /// <param name="lineColor">color to use</param>
        /// <param name="count">number of symbols to draw</param>
        private void DrawLineSymbolAndMoveHead(ref PixelPoint head, bool isVertical, in Symbol symbol, Color lineColor,
            int count)
        {
            PixelRect lineBounds = isVertical
                ? new PixelRect(head.X, head.Y, 1, count)
                : new PixelRect(head.X, head.Y, count, 1);
            PixelRect intersectLine = CurrentClip.Intersect(lineBounds);
            if (intersectLine.IsEmpty())
            {
                if (isVertical)
                    head = head.WithY(head.Y + count);
                else
                    head = head.WithX(head.X + count);
                return;
            }

            ushort lineStart = isVertical ? (ushort)intersectLine.Y : (ushort)intersectLine.X;
            ushort lineEnd = isVertical ? (ushort)intersectLine.Bottom : (ushort)intersectLine.Right;
            // adjust to the start of the intersected line
            if (isVertical)
                head = head.WithY(lineStart);
            else
                head = head.WithX(lineStart);

            var newPixel = new Pixel(new PixelForeground(symbol, lineColor));
            for (ushort i = lineStart; i < lineEnd; i++)
            {
                _pixelBuffer[head] = _pixelBuffer[head].Blend(newPixel);

                if (isVertical)
                    head = head.WithY(head.Y + 1);
                else
                    head = head.WithX(head.X + 1);
            }

            _consoleWindowImpl.DirtyRegions.AddRect(intersectLine);
        }


        private static BgraColor GetPixelColor(ReadOnlySpan<BgraColor> pixels, int x, int y, int stride,
            int bytesPerPixel)
        {
            int bytesPerRow = stride;
            int pixelsPerRow = bytesPerRow / bytesPerPixel;
            int offset = y * pixelsPerRow + x;

            BgraColor pixel = pixels[offset];

            // Handle RGB24 format (no alpha channel)
            if (bytesPerPixel == 3)
                pixel.A = 255;

            return pixel;
        }

        private static char GetQuadPixelCharacter(ReadOnlySpan<BgraColor> colors)
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
        private static Color GetForegroundColorForQuadPixel(char quadPixel, ReadOnlySpan<BgraColor> pixelColors)
        {
            if (pixelColors.Length != 4)
                throw new ArgumentException($"{nameof(pixelColors)} must have 4 elements.");

            // TODO: Some of these chars don't work in IBM Codepage
            BgraColor bgraColor = quadPixel switch
            {
                ' ' => BgraColor.Transparent,
                '▘' => pixelColors[0],
                '▝' => pixelColors[1],
                '▖' => pixelColors[2],
                '▗' => pixelColors[3],
                '▚' => CombineColors([pixelColors[0], pixelColors[3]]),
                '▞' => CombineColors([pixelColors[1], pixelColors[2]]),
                '▌' => CombineColors([pixelColors[0], pixelColors[2]]),
                '▐' => CombineColors([pixelColors[1], pixelColors[3]]),
                '▄' => CombineColors([pixelColors[2], pixelColors[3]]),
                '▀' => CombineColors([pixelColors[0], pixelColors[1]]),
                '▛' => CombineColors([pixelColors[0], pixelColors[1], pixelColors[2]]),
                '▜' => CombineColors([pixelColors[0], pixelColors[1], pixelColors[3]]),
                '▙' => CombineColors([pixelColors[0], pixelColors[2], pixelColors[3]]),
                '▟' => CombineColors([pixelColors[1], pixelColors[2], pixelColors[3]]),
                '█' => CombineColors(pixelColors),
                _ => throw new NotImplementedException()
            };

            return bgraColor.ToColor();
        }


        /// <summary>
        ///     Combine the colors for the black part of the quad pixel character.
        /// </summary>
        /// <param name="quadPixel"></param>
        /// <param name="pixelColors"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static Color GetBackgroundColorForQuadPixel(char quadPixel, ReadOnlySpan<BgraColor> pixelColors)
        {
            // TODO: Some of these chars don't work in IBM Codepage
            BgraColor bgraColor = quadPixel switch
            {
                ' ' => CombineColors(pixelColors),
                '▘' => CombineColors([pixelColors[1], pixelColors[2], pixelColors[3]]),
                '▝' => CombineColors([pixelColors[0], pixelColors[2], pixelColors[3]]),
                '▖' => CombineColors([pixelColors[0], pixelColors[1], pixelColors[3]]),
                '▗' => CombineColors([pixelColors[0], pixelColors[1], pixelColors[2]]),
                '▚' => CombineColors([pixelColors[1], pixelColors[2]]),
                '▞' => CombineColors([pixelColors[0], pixelColors[3]]),
                '▌' => CombineColors([pixelColors[1], pixelColors[3]]),
                '▐' => CombineColors([pixelColors[0], pixelColors[2]]),
                '▄' => CombineColors([pixelColors[0], pixelColors[1]]),
                '▀' => CombineColors([pixelColors[2], pixelColors[3]]),
                '▛' => pixelColors[3],
                '▜' => pixelColors[2],
                '▙' => pixelColors[1],
                '▟' => pixelColors[0],
                '█' => BgraColor.Transparent,
                _ => throw new NotImplementedException()
            };
            return bgraColor.ToColor();
        }


        private static BgraColor CombineColors(ReadOnlySpan<BgraColor> colors)
        {
            float accumR = 0, accumG = 0, accumB = 0;
            float accumAlpha = 0;

            foreach (ref readonly BgraColor color in colors)
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

            return new BgraColor(b, g, r, a);
        }

        /// <summary>
        ///     Cluster quad colors into a pattern (like: TTFF) based on relative closeness
        /// </summary>
        /// <param name="colors"></param>
        /// <returns>T or F for each color as a string</returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte GetColorsPattern(ReadOnlySpan<BgraColor> colors)
        {
            if (colors.Length != 4) throw new ArgumentException("Array must contain exactly 4 colors.");

            // Initial guess: two clusters with the first two colors as centers
            Span<BgraColor> clusterCenters = stackalloc BgraColor[2] { colors[0], colors[1] };
            Span<BgraColor> newClusterCenters = stackalloc BgraColor[2];
            Span<int> clusters = stackalloc int[4];

            for (int iteration = 0; iteration < 10; iteration++) // limit iterations to avoid infinite loop
            {
                // Assign colors to the closest cluster center
                for (int i = 0; i < colors.Length; i++)
                    clusters[i] = GetColorCluster(colors[i], clusterCenters);

                // Recalculate cluster centers
                newClusterCenters[0] = BgraColor.Transparent;
                newClusterCenters[1] = BgraColor.Transparent;
                for (int cluster = 0; cluster < 2; cluster++)
                {
                    // Calculate average for this cluster 
                    int totalRed = 0, totalGreen = 0, totalBlue = 0, totalAlpha = 0;
                    int count = 0;
                    bool allTransparent = true;

                    for (int i = 0; i < colors.Length; i++)
                        if (clusters[i] == cluster)
                        {
                            BgraColor color = colors[i];
                            totalRed += color.R;
                            totalGreen += color.G;
                            totalBlue += color.B;
                            totalAlpha += color.A;
                            count++;

                            if (color.A != 0)
                                allTransparent = false;
                        }

                    if (count > 0)
                    {
                        newClusterCenters[cluster].B = (byte)(totalBlue / count);
                        newClusterCenters[cluster].G = (byte)(totalGreen / count);
                        newClusterCenters[cluster].R = (byte)(totalRed / count);
                        newClusterCenters[cluster].A = (byte)(totalAlpha / count);
                    }

                    if (count == 4 && allTransparent)
                        return 0;
                }

                // Check for convergence
                bool converged = true;
                for (int i = 0; i < 2; i++)
                    if (!ColorEquals(clusterCenters[i], newClusterCenters[i]))
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

        private static bool ColorEquals(BgraColor c1, BgraColor c2)
        {
            return Unsafe.As<BgraColor, int>(ref c1) == Unsafe.As<BgraColor, int>(ref c2);
        }

        private static int GetColorCluster(BgraColor color, ReadOnlySpan<BgraColor> clusterCenters)
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

        private static double GetColorDistance(BgraColor c1, BgraColor c2)
        {
            int dr = c1.R - c2.R;
            int dg = c1.G - c2.G;
            int db = c1.B - c2.B;
            int da = c1.A - c2.A;

            return Math.Sqrt(dr * dr + dg * dg + db * db + da * da);
        }

        private static double GetColorBrightness(BgraColor color)
        {
            return 0.299 * color.R + 0.587 * color.G + 0.114 * color.B + color.A;
        }
    }
}