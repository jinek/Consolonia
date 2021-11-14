﻿using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Utilities;
using Avalonia.Visuals.Media.Imaging;
using Consolonia.Core.Drawing.PixelBuffer;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.Styles.Controls.Helpers;

namespace Consolonia.Core.Drawing
{
    internal class DrawingContextImpl : IDrawingContextImpl
    {
        private readonly Stack<Rect> _clipStack = new(100);
        private readonly IConsole _console;
        private readonly PixelBuffer.PixelBuffer _pixelBuffer;
        private readonly ConsoleWindow _consoleWindow;
        private readonly IVisualBrushRenderer _visualBrushRenderer;

        public DrawingContextImpl(ConsoleWindow consoleWindow, IVisualBrushRenderer visualBrushRenderer,
            PixelBuffer.PixelBuffer pixelBuffer)
        {
            _consoleWindow = consoleWindow;
            _visualBrushRenderer = visualBrushRenderer;
            _pixelBuffer = pixelBuffer;
            _clipStack.Push(pixelBuffer.Size);
            _console = AvaloniaLocator.Current.GetService<IConsole>();
        }

        private Rect _currentClip => _clipStack.Peek();

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
            return;

            //todo: when need this?
            Rect clip = _currentClip.Intersect(destRect);
            for (int x = 0; x < sourceRect.Width; x++)
            for (int y = 0; y < sourceRect.Height; y++)
            {
                var myRenderTarget = (RenderTarget)source.Item;


                int left = (int)(x + destRect.Left);
                int top = (int)(y + destRect.Top);
                clip.ExecuteWithClipping(new Point(left, top), () =>
                {
                    _pixelBuffer.Set(left, top, destPixel =>
                    {
                        return destPixel.Blend(myRenderTarget._bufferBuffer[(int)(x + sourceRect.Left),
                            (int)(y + sourceRect.Top)]);
                    });
                });
            }
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

            //todo: check opacity

            if (rect.Rect.IsEmpty) return; //todo: What to do here?
            Rect r = rect.Rect;

            if (brush is not null)
            {
                if (brush is not FourBitColorBrush backgroundBrush)
                {
                    ConsoloniaPlatform.RaiseNotSupported(9, brush, pen, rect, boxShadows);
                    return;
                }

                (double x, double y) = r.TopLeft.Transform(Transform);
                for (int i = 0; i < r.Width + (pen?.Thickness ?? 0); i++)
                for (int j = 0; j < r.Height + (pen?.Thickness ?? 0); j++)
                {
                    int px = (int)(x + i);
                    int py = (int)(y + j);
                    _currentClip.ExecuteWithClipping(new Point(px, py), () =>
                    {
                        _pixelBuffer.Set(px, py,
                            pixel => pixel.Blend(
                                new Pixel(
                                    new PixelBackground(backgroundBrush.Mode, backgroundBrush.Color))));
                    });
                }
            }

            if (pen is null or { Thickness: 0 } or {Brush: null}) return;

            DrawLineInternal(pen, new Line(r.TopLeft, false, (int)r.Width));
            DrawLineInternal(pen, new Line(r.BottomLeft, false, (int)r.Width));
            DrawLineInternal(pen, new Line(r.TopLeft, true, (int)r.Height));
            DrawLineInternal(pen, new Line(r.TopRight, true, (int)r.Height));
        }

        public void DrawText(IBrush foreground, Point origin, IFormattedTextImpl text)
        {
            //todo: restrictions
            DrawStringInternal(foreground, text.Text, origin);
        }

        public void DrawGlyphRun(IBrush foreground, GlyphRun glyphRun)
        {
            if (glyphRun.FontRenderingEmSize == 0) return;
            if (glyphRun.FontRenderingEmSize != 1)
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
            /*throw new InvalidOperationException();*/
            return new RenderTarget(_consoleWindow);
        }

        public void PushClip(Rect clip)
        {
            //todo: check if need to apply transform
            clip = new Rect(clip.Position.Transform(Transform), clip.BottomRight.Transform(Transform));
            _clipStack.Push(_currentClip.Intersect(clip));
        }

        public void PushClip(RoundedRect clip)
        {
            ConsoloniaPlatform.RaiseNotSupported(2);
            //PushClip(clip.Rect);
        }

        public void PopClip()
        {
            _clipStack.Pop();
        }

        public void PushOpacity(double opacity)
        {
            if (opacity == 1) return;
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

        public Matrix Transform { get; set; }

        private static ConsoleColor? ExtractConsoleColorOrNullWithPlatformCheck(IPen pen)
        {
            if (pen is not
            {
                Brush: FourBitColorBrush consoleColorBrush,
                Thickness: 1,
                DashStyle: null or { Dashes: { Count: 0 } },
                LineCap: PenLineCap.Flat,
                LineJoin: PenLineJoin.Miter
            })
            {
                ConsoloniaPlatform.RaiseNotSupported(6);
                return null;
            }

            if (consoleColorBrush.Mode == PixelBackgroundMode.Colored)
                return consoleColorBrush.Color;

            ConsoloniaPlatform.RaiseNotSupported(8);

            return null;
        }

        private void DrawLineInternal(IPen pen, Line line)
        {
            if (pen == null) throw new NotImplementedException("What is the situation about?");

            if (pen.Thickness == 0) return;

            Point head = line.PStart.Transform(Transform);

            if (pen.Brush is IConsoleCaretBrush consoleCaretBrush)
            {
                int penThickness = (int)pen.Thickness;
                if (!_currentClip.ExecuteWithClipping(head,
                    () =>
                    {
                        consoleCaretBrush.MoveCaret(head,
                            penThickness);
                    }))
                {
                    consoleCaretBrush.MoveCaret(null, penThickness);
                }
                return;
            }

            var extractConsoleColorCheckPlatformSupported = ExtractConsoleColorOrNullWithPlatformCheck(pen);
            if (extractConsoleColorCheckPlatformSupported == null)
                return;

            var consoleColor = (ConsoleColor)extractConsoleColorCheckPlatformSupported;

            byte marker = (byte)(line.Vertical ? 0b0010 : 0b0100);
            DrawPixelAndMoveHead(1); //beginning

            marker = (byte)(line.Vertical ? 0b1010 : 0b0101);
            DrawPixelAndMoveHead(line.Length - 1); //line

            marker = (byte)(line.Vertical ? 0b1000 : 0b0001);
            DrawPixelAndMoveHead(1); //ending 

            void DrawPixelAndMoveHead(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    _currentClip.ExecuteWithClipping(head, () =>
                    {
                        _pixelBuffer.Set((int)head.X, (int)head.Y,
                            pixel => pixel.Blend(new Pixel(marker, consoleColor)));
                    });
                    head = line.Vertical
                        ? head.WithY(head.Y + 1)
                        : head.WithX(head.X + 1);
                }
            }
        }

        private void DrawStringInternal(IBrush foreground, string str, Point origin = new())
        {
            if (foreground is not FourBitColorBrush { Mode: PixelBackgroundMode.Colored } consoleColorBrush)
            {
                ConsoloniaPlatform.RaiseNotSupported(4);
                return;
            }

            Point whereToDraw = origin.Transform(Transform);

            //todo: support surrogates
            for (int i = 0; i < str.Length; i++)
            {
                Point characterPoint = whereToDraw.Transform(Matrix.CreateTranslation(i, 0));
                _currentClip.ExecuteWithClipping(characterPoint, () =>
                {
                    var consolePixel = new Pixel(str[i], consoleColorBrush.Color);

                    _pixelBuffer.Set((int)characterPoint.X, (int)characterPoint.Y,
                        oldPixel => oldPixel.Blend(consolePixel));
                }); //todo: send to stack to avoid heap usages
            }
        }
    }
}