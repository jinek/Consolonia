using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Visuals.Media.Imaging;
using FormattedText = Consolonia.Core.Text.FormattedText;

namespace Consolonia.Core.Drawing
{
    internal class RenderInterface : IPlatformRenderInterface
    {
        private readonly IPlatformRenderInterface _platformRenderInterface;

        public RenderInterface(IPlatformRenderInterface platformRenderInterface)
        {
            _platformRenderInterface = platformRenderInterface;
        }

        public IFormattedTextImpl CreateFormattedText(
            string text,
            Typeface typeface,
            double fontSize,
            TextAlignment textAlignment,
            TextWrapping wrapping,
            Size constraint,
            IReadOnlyList<FormattedTextStyleSpan> spans)
        {
            return new FormattedText(text, textAlignment, wrapping, constraint, spans);
            //return _platformRenderInterface.CreateFormattedText(text, typeface, fontSize, textAlignment, wrapping, constraint, spans);
        }

        public IGeometryImpl CreateEllipseGeometry(Rect rect)
        {
            throw new NotImplementedException();
        }

        public IGeometryImpl CreateLineGeometry(Point p1, Point p2)
        {
            return Line.CreateMyLine(p1, p2);
            //return _platformRenderInterface.CreateLineGeometry(p1, p2);
        }

        public IGeometryImpl CreateRectangleGeometry(Rect rect)
        {
            return new Rectangle(rect);
            //return _platformRenderInterface.CreateRectangleGeometry(rect);
        }

        public IStreamGeometryImpl CreateStreamGeometry()
        {
            throw new NotImplementedException();
            //return _platformRenderInterface.CreateStreamGeometry();
        }

        public IGeometryImpl CreateGeometryGroup(FillRule fillRule, IReadOnlyList<Geometry> children)
        {
            throw new NotImplementedException();
        }

        public IGeometryImpl CreateCombinedGeometry(GeometryCombineMode combineMode, Geometry g1, Geometry g2)
        {
            throw new NotImplementedException();
        }

        public IRenderTarget CreateRenderTarget(IEnumerable<object> surfaces)
        {
            return new RenderTarget(surfaces);

            //return _platformRenderInterface.CreateRenderTarget(surfaces);
        }

        public IRenderTargetBitmapImpl CreateRenderTargetBitmap(PixelSize size, Vector dpi)
        {
            return _platformRenderInterface.CreateRenderTargetBitmap(size, dpi);
        }

        public IWriteableBitmapImpl CreateWriteableBitmap(PixelSize size, Vector dpi, PixelFormat format,
            AlphaFormat alphaFormat)
        {
            return _platformRenderInterface.CreateWriteableBitmap(size, dpi, format, alphaFormat);
        }

        public IBitmapImpl LoadBitmap(string fileName)
        {
            return _platformRenderInterface.LoadBitmap(fileName);
        }

        public IBitmapImpl LoadBitmap(Stream stream)
        {
            return _platformRenderInterface.LoadBitmap(stream);
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToWidth(Stream stream, int width,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            throw new NotImplementedException();
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToHeight(Stream stream, int height,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            throw new NotImplementedException();
        }

        public IWriteableBitmapImpl LoadWriteableBitmap(string fileName)
        {
            throw new NotImplementedException();
        }

        public IWriteableBitmapImpl LoadWriteableBitmap(Stream stream)
        {
            throw new NotImplementedException();
        }

        public IBitmapImpl LoadBitmapToWidth(Stream stream, int width,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            return _platformRenderInterface.LoadBitmapToWidth(stream, width, interpolationMode);
        }

        public IBitmapImpl LoadBitmapToHeight(Stream stream, int height,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            return _platformRenderInterface.LoadBitmapToHeight(stream, height, interpolationMode);
        }

        public IBitmapImpl ResizeBitmap(
            IBitmapImpl bitmapImpl,
            PixelSize destinationSize,
            BitmapInterpolationMode interpolationMode = BitmapInterpolationMode.HighQuality)
        {
            return _platformRenderInterface.ResizeBitmap(bitmapImpl, destinationSize, interpolationMode);
        }

        public IBitmapImpl LoadBitmap(
            PixelFormat format,
            AlphaFormat alphaFormat,
            IntPtr data,
            PixelSize size,
            Vector dpi,
            int stride)
        {
            return _platformRenderInterface.LoadBitmap(format, alphaFormat, data, size, dpi, stride);
        }

        public IGlyphRunImpl CreateGlyphRun(GlyphRun glyphRun, out double width)
        {
            return _platformRenderInterface.CreateGlyphRun(glyphRun, out width);
        }

        public bool SupportsIndividualRoundRects => false;

        public AlphaFormat DefaultAlphaFormat => throw new NotImplementedException();

        public PixelFormat DefaultPixelFormat => throw new NotImplementedException();
    }
}