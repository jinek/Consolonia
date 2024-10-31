using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Skia;
using SkiaSharp;

namespace Consolonia.Core.Drawing
{
    internal class BitmapImpl : IWriteableBitmapImpl
    {
        public BitmapImpl(int width, int height, PixelFormat format, AlphaFormat? alphaFormat = null)
        {
            Bitmap = new SKBitmap(new SKImageInfo(width, height, format.ToSkColorType(),
                alphaFormat?.ToSkAlphaType() ?? SKAlphaType.Unknown));
        }

        public BitmapImpl(SKBitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public BitmapImpl(Stream stream)
        {
            using var skStream = new SKManagedStream(stream);
            _bitmap = SKBitmap.Decode(skStream);
        }

        public BitmapImpl(string fileName)
        {
            Bitmap = SKBitmap.Decode(fileName);
        }

        public SKBitmap Bitmap => _bitmap;

        Vector IBitmapImpl.Dpi => new(96f, 96f);

        public PixelSize PixelSize => new(Bitmap.Width, (int)(Bitmap.Height * .55));

        public int Version => 1;

        public AlphaFormat? AlphaFormat => _bitmap.Info.AlphaType.ToAlphaFormat();

        public PixelFormat? Format => _bitmap.Info.ColorType.ToAvalonia();

        public void Dispose()
        {
            _bitmap.Dispose();
        }

        public IBitmapImpl Resize(PixelSize pixelSize, BitmapInterpolationMode interpolationMode)
        {
            var resized = new SKBitmap(pixelSize.Width, pixelSize.Height);
            using var canvas = new SKCanvas(resized);
            canvas.DrawBitmap(_bitmap, new SKRect(0, 0, pixelSize.Width, pixelSize.Height), new SKPaint { FilterQuality = interpolationMode.ToSKFilterQuality() });
            return new BitmapImpl(resized);
        }

        public void Save(string fileName, int? quality = 100)
        {
            SKEncodedImageFormat format = GetFormatFromFileName(fileName);
            using var image = SKImage.FromBitmap(_bitmap);
            using var data = image.Encode(format, quality ?? 100);
            using var stream = File.OpenWrite(fileName);
            data.SaveTo(stream);
        }

        public void Save(Stream stream, int? quality = 100)
        {
            SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg;
            using var image = SKImage.FromBitmap(_bitmap);
            using var data = image.Encode(format, quality ?? 100);
            data.SaveTo(stream);
        }

        public ILockedFramebuffer Lock()
        {
            throw new NotImplementedException();
        }

        public IBitmapImpl Resize(PixelSize pixelSize, BitmapInterpolationMode interpolationMode)
        {
            var resized = new SKBitmap(pixelSize.Width, pixelSize.Height);
            using (var canvas = new SKCanvas(resized))
            {
                canvas.DrawBitmap(Bitmap, new SKRect(0, 0, pixelSize.Width, pixelSize.Height),
                    new SKPaint { FilterQuality = interpolationMode.ToSKFilterQuality() });
            }

            return new BitmapImpl(resized);
        }

        private static SKEncodedImageFormat GetFormatFromFileName(string fileName)
        {
            if (Enum.TryParse<SKEncodedImageFormat>(Path.GetExtension(fileName).Trim('.'), ignoreCase: true, out var format))
                return format;
            return SKEncodedImageFormat.Jpeg;
        }
    }
}