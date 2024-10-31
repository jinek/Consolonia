using System;
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
            Bitmap = SKBitmap.Decode(skStream);
        }

        public BitmapImpl(string fileName)
        {
            Bitmap = SKBitmap.Decode(fileName);
        }

        public SKBitmap Bitmap { get; }

        Vector IBitmapImpl.Dpi => new(96f, 96f);

        public PixelSize PixelSize => new(Bitmap.Width, (int)(Bitmap.Height * .55));

        public int Version => 1;

        public AlphaFormat? AlphaFormat => Bitmap.Info.AlphaType.ToAlphaFormat();

        public PixelFormat? Format => Bitmap.Info.ColorType.ToAvalonia();

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        public void Save(string fileName, int? quality = 100)
        {
            SKEncodedImageFormat format = GetFormatFromFileName(fileName);
            using SKImage image = SKImage.FromBitmap(Bitmap);
            using SKData data = image.Encode(format, quality ?? 100);
            using FileStream stream = File.OpenWrite(fileName);
            data.SaveTo(stream);
        }

        public void Save(Stream stream, int? quality = 100)
        {
            var format = SKEncodedImageFormat.Jpeg;
            using SKImage image = SKImage.FromBitmap(Bitmap);
            using SKData data = image.Encode(format, quality ?? 100);
            data.SaveTo(stream);
        }

        public ILockedFramebuffer Lock()
        {
            throw new NotImplementedException();
        }

        public IBitmapImpl Resize(PixelSize pixelSize, BitmapInterpolationMode interpolationMode)
        {
            var resized = new SKBitmap(pixelSize.Width, pixelSize.Height);
            using var canvas = new SKCanvas(resized);
            canvas.DrawBitmap(Bitmap, new SKRect(0, 0, pixelSize.Width, pixelSize.Height),
                new SKPaint { FilterQuality = interpolationMode.ToSKFilterQuality() });
            return new BitmapImpl(resized);
        }

        private static SKEncodedImageFormat GetFormatFromFileName(string fileName)
        {
            if (Enum.TryParse<SKEncodedImageFormat>(Path.GetExtension(fileName).Trim('.'), true,
                    out SKEncodedImageFormat format))
                return format;
            return SKEncodedImageFormat.Jpeg;
        }
    }
}