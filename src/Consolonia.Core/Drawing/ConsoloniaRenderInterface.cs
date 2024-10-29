using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Media.TextFormatting;
using Avalonia.Platform;
using Consolonia.Core.Text;

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
        }

        public IWriteableBitmapImpl CreateWriteableBitmap(PixelSize size, Vector dpi, PixelFormat format,
            AlphaFormat alphaFormat)
        {
            throw new NotImplementedException();
        }

        public IBitmapImpl LoadBitmap(string fileName)
        {
            throw new NotImplementedException();
        }

        public IBitmapImpl LoadBitmap(Stream stream)
        {
            throw new NotImplementedException();
        }

        IWriteableBitmapImpl IPlatformRenderInterface.LoadWriteableBitmapToHeight(Stream stream, int height,
            BitmapInterpolationMode interpolationMode)
        {
            throw new NotImplementedException();
        }

        public IWriteableBitmapImpl LoadWriteableBitmapToWidth(Stream stream, int width,
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

        IBitmapImpl IPlatformRenderInterface.LoadBitmapToWidth(Stream stream, int width,
            BitmapInterpolationMode interpolationMode)
        {
            throw new NotImplementedException();
        }

        IBitmapImpl IPlatformRenderInterface.LoadBitmapToHeight(Stream stream, int height,
            BitmapInterpolationMode interpolationMode)
        {
            throw new NotImplementedException();
        }

        IBitmapImpl IPlatformRenderInterface.ResizeBitmap(IBitmapImpl bitmapImpl, PixelSize destinationSize,
            BitmapInterpolationMode interpolationMode)
        {
            throw new NotImplementedException();
        }
        
        public IBitmapImpl LoadBitmap(
            PixelFormat format,
            AlphaFormat alphaFormat,
            IntPtr data,
            PixelSize size,
            Vector dpi,
            int stride)
        {
            throw new NotImplementedException();
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