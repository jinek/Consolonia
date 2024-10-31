using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using Consolonia.Core.Text;
using SkiaSharp;

namespace Consolonia.Core.Drawing
{
    internal class DrawingContextImpl : IDrawingContextImpl
    {
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
            using var bitmap = new SKBitmap((int)targetRect.Width, (int)targetRect.Height);
            using var canvas = new SKCanvas(bitmap);
            canvas.DrawBitmap(bmp.Bitmap, new SKRect(0, 0, (float)targetRect.Width, (float)targetRect.Height), new SKPaint { FilterQuality = SKFilterQuality.Medium });

            // Rect clip = CurrentClip.Intersect(destRect);
            var width = bitmap.Info.Width;
            var height = bitmap.Info.Height;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int px = ((int)targetRect.TopLeft.X + x);
                    int py = ((int)targetRect.TopLeft.Y + y);
                    var skColor = bitmap.GetPixel(x, y);
                    var color = Color.FromRgb(skColor.Red, skColor.Green, skColor.Blue);
                    var imagePixel = new Pixel('█', color);
                    CurrentClip.ExecuteWithClipping(new Point(px, py), () =>
                    {
                        _pixelBuffer.Set(new PixelBufferCoordinate((ushort)px, (ushort)py), (existingPixel, _) => existingPixel.Blend(imagePixel), imagePixel.Background.Color);
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
                                (pixel, bb) => { return pixel.Blend(new Pixel(new PixelBackground(bb.Mode, bb.Color))); },
                                backgroundBrush);
                        });
                    }
            }

            if (pen is null or { Thickness: 0 }
                or { Brush: null }) return;

            DrawLineInternal(pen, new Line(r.TopLeft, false, (int)r.Width));
            DrawLineInternal(pen, new Line(r.BottomLeft, false, (int)r.Width));
            DrawLineInternal(pen, new Line(r.TopLeft, true, (int)r.Height));
            DrawLineInternal(pen, new Line(r.TopRight, true, (int)r.Height));
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

            string charactersDoDraw =
                string.Concat(glyphRunImpl.GlyphIndices.Select(us => (char)us).ToArray());
            DrawStringInternal(foreground, charactersDoDraw, glyphRun.GlyphTypeface);
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

        private static Color? ExtractColorOrNullWithPlatformCheck(IPen pen, out LineStyle? lineStyle)
        {
            lineStyle = null;
            if (pen is not
                {
                    Brush: ConsoleBrush or LineBrush or ImmutableSolidColorBrush,
                    Thickness: 1,
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

            ConsoleBrush consoleColorBrush = ConsoleBrush.FromBrush(pen.Brush);

            switch (consoleColorBrush.Mode)
            {
                case PixelBackgroundMode.Colored:
                    return consoleColorBrush.Color;
                case PixelBackgroundMode.Transparent:
                    return null;
                case PixelBackgroundMode.Shaded:
                    ConsoloniaPlatform.RaiseNotSupported(8);
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pen));
            }
        }

        private void DrawLineInternal(IPen pen, Line line)
        {
            if (pen.Thickness == 0) return;

            if (!Transform.NoRotation()) ConsoloniaPlatform.RaiseNotSupported(16);

            line = (Line)line.WithTransform(Transform);

            Point head = line.PStart;

            if (pen.Brush is MoveConsoleCaretToPositionBrush)
            {
                CurrentClip.ExecuteWithClipping(head,
                    () => { _pixelBuffer.Set((PixelBufferCoordinate)head, pixel => pixel.Blend(new Pixel(true))); });

                return;
            }

            var extractColorCheckPlatformSupported =
                ExtractColorOrNullWithPlatformCheck(pen, out var lineStyle);
            if (extractColorCheckPlatformSupported == null)
                return;

            var consoleColor = (Color)extractColorCheckPlatformSupported;

            byte pattern = (byte)(line.Vertical ? 0b0010 : 0b0100);
            DrawPixelAndMoveHead(1); //beginning

            pattern = (byte)(line.Vertical ? 0b1010 : 0b0101);
            DrawPixelAndMoveHead(line.Length - 1); //line

            pattern = (byte)(line.Vertical ? 0b1000 : 0b0001);
            DrawPixelAndMoveHead(1); //ending 
            return;

            void DrawPixelAndMoveHead(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    CurrentClip.ExecuteWithClipping(head, () =>
                    {
                        // ReSharper disable once AccessToModifiedClosure todo: pass as a parameter
                        _pixelBuffer.Set((PixelBufferCoordinate)head,
                            (pixel, mcC) => pixel.Blend(new Pixel(DrawingBoxSymbol.UpRightDownLeftFromPattern(
                                mcC.pattern,
                                lineStyle ?? LineStyle.SingleLine), mcC.consoleColor)),
                            (pattern, consoleColor));
                    });
                    head = line.Vertical
                        ? head.WithY(head.Y + 1)
                        : head.WithX(head.X + 1);
                }
            }
        }

        private void DrawStringInternal(IBrush foreground, string str, IGlyphTypeface typeface, Point origin = new())
        {
            foreground = ConsoleBrush.FromBrush(foreground);
            if (foreground is not ConsoleBrush { Mode: PixelBackgroundMode.Colored } consoleColorBrush)
            {
                ConsoloniaPlatform.RaiseNotSupported(4);
                return;
            }

            //if (!Transform.IsTranslateOnly()) ConsoloniaPlatform.RaiseNotSupported(15);

            Point whereToDraw = origin.Transform(Transform);
            int currentXPosition = 0;

            //todo: support surrogates
            foreach (char c in str)
            {
                Point characterPoint = whereToDraw.Transform(Matrix.CreateTranslation(currentXPosition++, 0));
                Color foregroundColor = consoleColorBrush.Color;

                switch (c)
                {
                    case '\t':
                        {
                            const int tabSize = 8;
                            var consolePixel = new Pixel(' ', foregroundColor);
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
                    case '\n':
                        {
                            /* it's not clear if we need to draw anything. Cursor can be placed at the end of the line
                             var consolePixel =  new Pixel(' ', foregroundColor);

                            _pixelBuffer.Set((PixelBufferCoordinate)characterPoint,
                                (oldPixel, cp) => oldPixel.Blend(cp), consolePixel);*/
                        }
                        break;
                    case '\u200B':
                        currentXPosition--;
                        break;
                    default:
                        {
                            var consolePixel = new Pixel(c, foregroundColor, typeface.Style, typeface.Weight);
                            CurrentClip.ExecuteWithClipping(characterPoint, () =>
                            {
                                _pixelBuffer.Set((PixelBufferCoordinate)characterPoint,
                                    (oldPixel, cp) => oldPixel.Blend(cp), consolePixel);
                            }
                            );
                        }
                        break;
                }
            }
        }
    }
}