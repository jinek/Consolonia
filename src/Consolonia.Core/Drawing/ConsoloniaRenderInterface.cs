using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public IGeometryImpl CreateGeometryGroup(FillRule fillRule, IReadOnlyList<IGeometryImpl> children)
        {
            throw new NotImplementedException();
        }

        public IGeometryImpl CreateCombinedGeometry(GeometryCombineMode combineMode, IGeometryImpl g1, IGeometryImpl g2)
        {
            throw new NotImplementedException();
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

        public IWriteableBitmapImpl CreateWriteableBitmap(PixelSize size, Vector dpi, PixelFormat format, AlphaFormat alphaFormat)
        {
            return new BitmapImpl(size.Width, size.Height, format, alphaFormat);
        }

        public IBitmapImpl LoadBitmap(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return new BitmapImpl(stream);
            }
        }

        public IBitmapImpl LoadBitmap(Stream stream)
        {
            return new BitmapImpl(stream);
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                var originalBitmap = SKBitmap.Decode(skStream);
                var width = (int)(height * ((double)originalBitmap.Width / originalBitmap.Height));
                var resizedBitmap = originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                var originalBitmap = SKBitmap.Decode(skStream);
                var height = (int)(width * ((double)originalBitmap.Height / originalBitmap.Width));
                var resizedBitmap = originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IWriteableBitmapImpl LoadWriteableBitmap(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return LoadWriteableBitmap(stream);
            }
        }

        public IWriteableBitmapImpl LoadWriteableBitmap(Stream stream)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                var originalBitmap = SKBitmap.Decode(skStream);
                return new BitmapImpl(originalBitmap);
            }
        }

        public IBitmapImpl LoadBitmapToWidth(Stream stream, int width, BitmapInterpolationMode interpolationMode)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                var originalBitmap = SKBitmap.Decode(skStream);
                var height = (int)(width * ((double)originalBitmap.Height / originalBitmap.Width));
                var resizedBitmap = originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IBitmapImpl LoadBitmapToHeight(Stream stream, int height, BitmapInterpolationMode interpolationMode)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                var originalBitmap = SKBitmap.Decode(skStream);
                var width = (int)(height * ((double)originalBitmap.Width / originalBitmap.Height));
                var resizedBitmap = originalBitmap.Resize(new SKImageInfo(width, height), (SKFilterQuality)interpolationMode);
                return new BitmapImpl(resizedBitmap);
            }
        }

        public IBitmapImpl ResizeBitmap(IBitmapImpl bitmapImpl, PixelSize destinationSize, BitmapInterpolationMode interpolationMode)
        {
            var consoleBitmap = (BitmapImpl)bitmapImpl;
            return consoleBitmap.Resize(destinationSize, interpolationMode);
        }

        public IBitmapImpl LoadBitmap(PixelFormat format, AlphaFormat alphaFormat, IntPtr data, PixelSize size, Vector dpi, int stride)
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

        public bool SupportsIndividualRoundRects => false;

        public AlphaFormat DefaultAlphaFormat => throw new NotImplementedException();

        public PixelFormat DefaultPixelFormat => throw new NotImplementedException();
    }
}