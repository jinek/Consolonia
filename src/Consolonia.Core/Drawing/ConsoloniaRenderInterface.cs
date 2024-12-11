using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Text;
using SkiaSharp;

namespace Consolonia.Core.Drawing
{
    internal class ConsoloniaRenderInterface : IPlatformRenderInterface
    {
        public IGeometryImpl CreateEllipseGeometry(Rect rect)
        {
            throw new NotImplementedException();
        }

        public IGeometryImpl CreateLineGeometry(Point p1, Point p2)
        {
            return Line.CreateMyLine(p1, p2);
        }

        public IGeometryImpl CreateRectangleGeometry(Rect rect)
        {
            return new Rectangle(rect);
        }

        public IStreamGeometryImpl CreateStreamGeometry()
        {
            return new StreamGeometryImpl();
        }

        public IGeometryImpl CreateGeometryGroup(FillRule fillRule, IReadOnlyList<IGeometryImpl> children)
        {
            throw new NotImplementedException();
        }

        public IGeometryImpl CreateCombinedGeometry(GeometryCombineMode combineMode, IGeometryImpl g1, IGeometryImpl g2)
        {
            // this is handcrafted to only combine single thickness line strokes for borders.
            // This needs to be much more robust to handle general cases.

            if (combineMode != GeometryCombineMode.Exclude)
                throw new NotImplementedException("Only GeometryCombineMode.Exclude is supported");

            if (g1 is not StreamGeometryImpl stream1 || g2 is not StreamGeometryImpl stream2)
                throw new ArgumentException("Only StreamGeometryImpl is supported");

            Debug.WriteLine($"=====");
            Debug.WriteLine($"Stream1 Bounds: {stream1.Bounds}");
            Debug.WriteLine($"Stream2 Bounds: {stream2.Bounds}");

            var newGeometry = CreateStreamGeometry();
            using (var ctx = newGeometry.Open())
            {
                var hasLeftStroke = stream2.Bounds.X == 1;
                var hasTopStroke = stream2.Bounds.Y == 1;
                var hasRightStroke = (stream1.Bounds.Width - stream2.Bounds.Width) == stream2.Bounds.X + 1;
                var hasBottomStroke = (stream1.Bounds.Height - stream2.Bounds.Height) == stream2.Bounds.Y + 1;

                // add "null" strokes to establish boundries of box.
                var topLeft = stream1.Bounds.TopLeft;
                var topRight = stream1.Bounds.TopRight;
                var bottomLeft = stream1.Bounds.BottomLeft;
                var bottomRight = stream1.Bounds.BottomRight;
                Debug.WriteLine($"Stream1 {stream1.Bounds.Width}x{stream1.Bounds.Height}");
                Debug.WriteLine($"Stream2 {stream2.Bounds.Width}x{stream2.Bounds.Height}");
                AddStroke(ctx, topLeft, topLeft);
                AddStroke(ctx, bottomRight, bottomRight);

                #region LAYOUT_PATCHES
                // Layout patches. This is a mess, see avalonia bug https://github.com/AvaloniaUI/Avalonia/issues/17752
                if (hasBottomStroke && !hasTopStroke && (hasLeftStroke || hasRightStroke))
                {
                    bottomLeft = AdjustY(bottomLeft, -1);
                    bottomRight = AdjustY(bottomRight, -1);
                }

                if (hasBottomStroke && !hasTopStroke && !hasLeftStroke && !hasRightStroke)
                {
                    bottomLeft = AdjustY(bottomLeft, -.99); // Sigh. -1 doesn't work, but -.99 does.
                    bottomRight = AdjustY(bottomRight, -.99);
                }
                
                if (hasRightStroke && !hasLeftStroke)
                {
                    topRight = AdjustX(topRight, -1);
                    bottomRight = AdjustX(bottomRight, -1);
                }
                
                if (hasTopStroke && hasBottomStroke)
                {
                    bottomLeft = AdjustY(bottomLeft, -1);
                    bottomRight = AdjustY(bottomRight, -1);
                }

                if (hasLeftStroke && hasRightStroke)
                {
                    topRight= AdjustX(topRight, -1);
                    bottomRight = AdjustX(bottomRight, -1);
                }
                #endregion

                if (hasTopStroke)
                    AddStroke(ctx, topLeft, topRight);
                if (hasRightStroke)
                    AddStroke(ctx, topRight, bottomRight);
                if (hasBottomStroke)
                    AddStroke(ctx, bottomLeft, bottomRight);
                if (hasLeftStroke)
                    AddStroke(ctx, topLeft, bottomLeft);

                Point AdjustXY(Point p, double deltaX, int deltaY) => new Point(p.X + deltaX, p.Y + deltaY);
                Point AdjustX(Point p, double deltaX) => p.WithX(p.X + deltaX);
                Point AdjustY(Point p, double deltaY) => p.WithY(p.Y + deltaY);
            }

            var newg = newGeometry as StreamGeometryImpl;
            foreach (var stroke in newg.Strokes)
            {
                Debug.WriteLine($"New.Stroke: {stroke.PStart} - {stroke.PEnd}");
            }

            return newGeometry;
        }

        private static void AddStroke(IStreamGeometryContextImpl ctx, Point start, Point end)
        {
            ctx.BeginFigure(start, false);
            ctx.LineTo(end);
            ctx.EndFigure(true);
        }

        public IGeometryImpl BuildGlyphRunGeometry(GlyphRun glyphRun)
        {
            throw new NotImplementedException();
        }

        public IRenderTargetBitmapImpl CreateRenderTargetBitmap(PixelSize size, Vector dpi)
        {
            throw new NotImplementedException();
            // return new ConsoleRenderTargetBitmapImpl(size, dpi);
        }

        public IWriteableBitmapImpl CreateWriteableBitmap(PixelSize size, Vector dpi, PixelFormat format,
            AlphaFormat alphaFormat)
        {
            return new BitmapImpl(size.Width, size.Height, format, alphaFormat);
        }

        public IBitmapImpl LoadBitmap(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return new BitmapImpl(stream);
            }
        }

        public IBitmapImpl LoadBitmap(Stream stream)
        {
            return new BitmapImpl(stream);
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToHeight(Stream stream, int height,
            BitmapInterpolationMode interpolationMode)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                SKBitmap originalBitmap = SKBitmap.Decode(skStream);
                int width = (int)(height * ((double)originalBitmap.Width / originalBitmap.Height));
                SKBitmap resizedBitmap =
                    originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToWidth(Stream stream, int width,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                SKBitmap originalBitmap = SKBitmap.Decode(skStream);
                int height = (int)(width * ((double)originalBitmap.Height / originalBitmap.Width));
                SKBitmap resizedBitmap =
                    originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IWriteableBitmapImpl LoadWriteableBitmap(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                return LoadWriteableBitmap(stream);
            }
        }

        public IWriteableBitmapImpl LoadWriteableBitmap(Stream stream)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                SKBitmap originalBitmap = SKBitmap.Decode(skStream);
                return new BitmapImpl(originalBitmap);
            }
        }

        public IBitmapImpl LoadBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                SKBitmap originalBitmap = SKBitmap.Decode(skStream);
                int height = (int)(width * ((double)originalBitmap.Height / originalBitmap.Width));
                SKBitmap resizedBitmap =
                    originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IBitmapImpl LoadBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                SKBitmap originalBitmap = SKBitmap.Decode(skStream);
                int width = (int)(height * ((double)originalBitmap.Width / originalBitmap.Height));
                SKBitmap resizedBitmap =
                    originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IBitmapImpl ResizeBitmap(IBitmapImpl bitmapImpl, PixelSize destinationSize,
            BitmapInterpolationMode interpolationMode)
        {
            var consoleBitmap = (BitmapImpl)bitmapImpl;
            return consoleBitmap.Resize(destinationSize, interpolationMode);
        }

        public IBitmapImpl LoadBitmap(PixelFormat format, AlphaFormat alphaFormat, IntPtr data, PixelSize size,
            Vector dpi, int stride)
        {
            var info = new SKImageInfo(size.Width, size.Height, SKColorType.Rgb888x, SKAlphaType.Opaque);
            var bitmap = new SKBitmap();
            bitmap.InstallPixels(info, data, stride);
            return new BitmapImpl(bitmap);
        }


        public IGlyphRunImpl CreateGlyphRun(IGlyphTypeface glyphTypeface, double fontRenderingEmSize,
            IReadOnlyList<GlyphInfo> glyphInfos,
            Point baselineOrigin)
        {
            return new GlyphRunImpl(glyphTypeface, glyphInfos, baselineOrigin);
        }

        public IPlatformRenderInterfaceContext CreateBackendContext(IPlatformGraphicsContext graphicsApiContext)
        {
            if (graphicsApiContext != null)
                throw new NotImplementedException("Investigate this cases");
            return new ConsoloniaPlatformRenderInterfaceContext();
        }

        public bool IsSupportedBitmapPixelFormat(PixelFormat format)
        {
            throw new NotImplementedException();
        }

        public IPlatformRenderInterfaceRegion CreateRegion()
        {
            throw new NotImplementedException();
        }

        public bool SupportsIndividualRoundRects => false;

        public AlphaFormat DefaultAlphaFormat => throw new NotImplementedException();

        public PixelFormat DefaultPixelFormat => throw new NotImplementedException();

        public bool SupportsRegions => false;
    }
}