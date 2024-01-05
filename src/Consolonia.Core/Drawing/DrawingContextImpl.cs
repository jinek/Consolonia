using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Utilities;
using Avalonia.Visuals.Media.Imaging;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.InternalHelpers;
using FormattedText = Consolonia.Core.Text.FormattedText;

namespace Consolonia.Core.Drawing
{
    internal class DrawingContextImpl : IDrawingContextImpl
    {
        private readonly Stack<Rect> _clipStack = new(100);
        private readonly ConsoleWindow _consoleWindow;
        private readonly PixelBuffer _pixelBuffer;
        private readonly IVisualBrushRenderer _visualBrushRenderer;
        private Matrix _postTransform = Matrix.Identity;
        private Matrix _transform;

        public DrawingContextImpl(ConsoleWindow consoleWindow, IVisualBrushRenderer visualBrushRenderer,
            PixelBuffer pixelBuffer)
        {
            _consoleWindow = consoleWindow;
            _visualBrushRenderer = visualBrushRenderer;
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

        public void DrawBitmap(
            IRef<IBitmapImpl> source,
            double opacity,
            Rect sourceRect,
            Rect destRect,
            BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode.Default)
        {
            /*
            //  prototype
             Rect clip = _currentClip.Intersect(destRect);
            for (int x = 0; x < sourceRect.Width; x++)
            for (int y = 0; y < sourceRect.Height; y++)
            {
                var myRenderTarget = (RenderTarget)source.Item;

                int left = (int)(x + destRect.Left);
                int top = (int)(y + destRect.Top);
                clip.ExecuteWithClipping(new Point(left, top), () =>
                {
                    _pixelBuffer.Set(new PixelBufferCoordinate((ushort)left, (ushort)top), destPixel =>
                    {
                        return destPixel.Blend(
                            myRenderTarget._bufferBuffer[new PixelBufferCoordinate((ushort)(x + sourceRect.Left),
                                (ushort)(y + sourceRect.Top))]);
                    });
                });
            }*/
        }

        public void DrawBitmap(IRef<IBitmapImpl> source, IBrush opacityMask, Rect opacityMaskRect, Rect destRect)
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

            if (rect.Rect.IsEmpty) return;
            Rect r = rect.Rect;

            if (brush is not null)
            {
                if (brush is IVisualBrush visualBrush)
                {
                    try
                    {
                        _postTransform = Transform;
                        _visualBrushRenderer.RenderVisualBrush(this, visualBrush);
                    }
                    finally
                    {
                        _postTransform = Matrix.Identity;
                    }

                    return;
                }

                if (brush is not FourBitColorBrush backgroundBrush)
                {
                    ConsoloniaPlatform.RaiseNotSupported(9, brush, pen, rect, boxShadows);
                    return;
                }

                {
                    Rect r2 = r.TransformToAABB(Transform);

                    (double x, double y) = r2.TopLeft;
                    for (int i = 0; i < r2.Width + (pen?.Thickness ?? 0); i++)
                    for (int j = 0; j < r2.Height + (pen?.Thickness ?? 0); j++)
                    {
                        int px = (int)(x + i);
                        int py = (int)(y + j);
                        CurrentClip.ExecuteWithClipping(new Point(px, py), () =>
                        {
                            _pixelBuffer.Set(new PixelBufferCoordinate((ushort)px, (ushort)py),
                                (pixel, bb) => pixel.Blend(
                                    new Pixel(
                                        new PixelBackground(bb.Mode, bb.Color))), backgroundBrush);
                        });
                    }
                }
            }

            if (pen is null or { Thickness: 0 } or { Brush: null }) return;

            DrawLineInternal(pen, new Line(r.TopLeft, false, (int)r.Width));
            DrawLineInternal(pen, new Line(r.BottomLeft, false, (int)r.Width));
            DrawLineInternal(pen, new Line(r.TopLeft, true, (int)r.Height));
            DrawLineInternal(pen, new Line(r.TopRight, true, (int)r.Height));
        }

        public void DrawEllipse(IBrush brush, IPen pen, Rect rect)
        {
            throw new NotImplementedException();
        }

        public void DrawText(IBrush foreground, Point origin, IFormattedTextImpl text)
        {
            var formattedText = (FormattedText)text;

            for (int row = 0; row < formattedText.SkiaLines.Count; row++)
            {
                FormattedText.AvaloniaFormattedTextLine line = formattedText.SkiaLines[row];
                float x = formattedText.TransformX((float)origin.X, line.Width);
                string subString = text.Text.Substring(line.Start, line.Length);
                DrawStringInternal(foreground, subString, new Point(x, origin.Y + row), line.Start,
                    formattedText.ForegroundBrushes.Any() ? formattedText.ForegroundBrushes : null);
            }
        }

        public void DrawGlyphRun(IBrush foreground, GlyphRun glyphRun)
        {
            if (glyphRun.FontRenderingEmSize.IsNearlyEqual(0)) return;
            if (!glyphRun.FontRenderingEmSize.IsNearlyEqual(1))
            {
                ConsoloniaPlatform.RaiseNotSupported(3);
                return;
            }

            string charactersDoDraw =
                string.Concat(glyphRun.GlyphIndices.Select(us => (char)us).ToArray());
            DrawStringInternal(foreground, charactersDoDraw);
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
            ConsoloniaPlatform.RaiseNotSupported(2);
        }

        public void PopClip()
        {
            _clipStack.Pop();
        }

        public void PushOpacity(double opacity)
        {
            if (opacity.IsNearlyEqual(1)) return;
            ConsoloniaPlatform.RaiseNotSupported(7);
        }

        public void PopOpacity()
        {
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

        public void PushBitmapBlendMode(BitmapBlendingMode blendingMode)
        {
            throw new NotImplementedException();
        }

        public void PopBitmapBlendMode()
        {
            throw new NotImplementedException();
        }

        public void Custom(ICustomDrawOperation custom)
        {
            throw new NotImplementedException();
        }

        public Matrix Transform
        {
            get => _transform;
            set => _transform = value * _postTransform;
        }

        private static ConsoleColor? ExtractConsoleColorOrNullWithPlatformCheck(IPen pen, out LineStyle? lineStyle)
        {
            lineStyle = null;
            if (pen is not
            {
                Brush: FourBitColorBrush or LineBrush { Brush: FourBitColorBrush },
                Thickness: 1,
                DashStyle: null or { Dashes: { Count: 0 } },
                LineCap: PenLineCap.Flat,
                LineJoin: PenLineJoin.Miter
            })
            {
                ConsoloniaPlatform.RaiseNotSupported(6);
                return null;
            }

            if (pen.Brush is not FourBitColorBrush consoleColorBrush)
            {
                var lineBrush = (LineBrush)pen.Brush;
                consoleColorBrush = (FourBitColorBrush)lineBrush.Brush;
                lineStyle = lineBrush.LineStyle;
            }

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

            var extractConsoleColorCheckPlatformSupported =
                ExtractConsoleColorOrNullWithPlatformCheck(pen, out var lineStyle);
            if (extractConsoleColorCheckPlatformSupported == null)
                return;

            var consoleColor = (ConsoleColor)extractConsoleColorCheckPlatformSupported;

            byte pattern = (byte)(line.Vertical ? 0b0010 : 0b0100);
            DrawPixelAndMoveHead(1); //beginning

            pattern = (byte)(line.Vertical ? 0b1010 : 0b0101);
            DrawPixelAndMoveHead(line.Length - 1); //line

            pattern = (byte)(line.Vertical ? 0b1000 : 0b0001);
            DrawPixelAndMoveHead(1); //ending 

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

        private void DrawStringInternal(IBrush foreground, string str, Point origin = new(), int startIndex = 0,
            List<KeyValuePair<FormattedText.FBrushRange, IBrush>> additionalBrushes = null)
        {
            if (foreground is not FourBitColorBrush { Mode: PixelBackgroundMode.Colored } consoleColorBrush)
            {
                ConsoloniaPlatform.RaiseNotSupported(4);
                return;
            }

            if (!Transform.IsTranslateOnly()) ConsoloniaPlatform.RaiseNotSupported(15);

            Point whereToDraw = origin.Transform(Transform);
            int currentXPosition = 0;

            //todo: support surrogates
            for (int i = 0; i < str.Length; i++)
            {
                Point characterPoint = whereToDraw.Transform(Matrix.CreateTranslation(currentXPosition++, 0));
                // ReSharper disable AccessToModifiedClosure
                CurrentClip.ExecuteWithClipping(characterPoint, () =>
                {
                    ConsoleColor foregroundColor = consoleColorBrush.Color;
                    if (additionalBrushes != null)
                    {
                        (FormattedText.FBrushRange _, IBrush brush) = additionalBrushes.FirstOrDefault(pair =>
                        {
                            // ReSharper disable once AccessToModifiedClosure //todo: pass as a parameter
                            int globalIndex = i + startIndex;
                            (FormattedText.FBrushRange key, _) = pair;
                            return key.StartIndex <= globalIndex && globalIndex < key.EndIndex;
                        });

                        if (brush != null)
                        {
                            if (brush is not FourBitColorBrush { Mode: PixelBackgroundMode.Colored } additionalBrush)
                            {
                                ConsoloniaPlatform.RaiseNotSupported(11);
                                return;
                            }

                            foregroundColor = additionalBrush.Color;
                        }
                    }

                    char character = str[i];

                    switch (character)
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
                                    _pixelBuffer.Set((PixelBufferCoordinate)characterPoint.WithX(characterPoint.X + j),
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
                        default:
                        {
                            var consolePixel = new Pixel(character, foregroundColor);

                            _pixelBuffer.Set((PixelBufferCoordinate)characterPoint,
                                (oldPixel, cp) => oldPixel.Blend(cp), consolePixel);
                        }
                            break;
                    }
                });
                // ReSharper restore AccessToModifiedClosure
            }
        }
    }
}