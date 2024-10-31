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
        private SKBitmap _bitmap;

        public BitmapImpl(int width, int height, PixelFormat format, AlphaFormat? alphaFormat = null)
        {
            _bitmap = new SKBitmap(new SKImageInfo(width, height, format.ToSkColorType(), alphaFormat?.ToSkAlphaType() ?? SKAlphaType.Unknown));
        }

        public BitmapImpl(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public BitmapImpl(Stream stream)
        {
            using (var skStream = new SKManagedStream(stream))
            {
                _bitmap = SKBitmap.Decode(skStream);
            }
        }

        public BitmapImpl(string fileName)
        {
            _bitmap = SKBitmap.Decode(fileName);
        }

        public SKBitmap Bitmap { get { return _bitmap; } }

        Vector IBitmapImpl.Dpi => new Vector(96f, 96f);

        public PixelSize PixelSize => new PixelSize(_bitmap.Width, (int)(_bitmap.Height * .55));

        public int Version => 1;

        public AlphaFormat? AlphaFormat { get => _bitmap.Info.AlphaType.ToAlphaFormat(); }

        public PixelFormat? Format { get => _bitmap.Info.ColorType.ToAvalonia();  }

        public void Dispose()
        {
            _bitmap.Dispose();
        }

        public IBitmapImpl Resize(PixelSize pixelSize, BitmapInterpolationMode interpolationMode)
        {
            var resized = new SKBitmap(pixelSize.Width, pixelSize.Height);
            using (var canvas = new SKCanvas(resized))
            {
                canvas.DrawBitmap(_bitmap, new SKRect(0, 0, pixelSize.Width, pixelSize.Height), new SKPaint { FilterQuality = interpolationMode.ToSKFilterQuality()});
            }
            return new BitmapImpl(resized);
        }

        public void Save(string fileName, int? quality = 100)
        {
            SKEncodedImageFormat format = GetFormatFromFileName(fileName);
            using (var image = SKImage.FromBitmap(_bitmap))
            using (var data = image.Encode(format, quality.Value))
            using (var stream = File.OpenWrite(fileName))
            {
                data.SaveTo(stream);
            }
        }

        public void Save(Stream stream, int? quality = 100)
        {
            SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg;
            using (var image = SKImage.FromBitmap(_bitmap))
            using (var data = image.Encode(format, quality.Value))
            {
                data.SaveTo(stream);
            }
        }
        public ILockedFramebuffer Lock()
        {
            throw new NotImplementedException();
        }

        private static SKEncodedImageFormat GetFormatFromFileName(string fileName)
        {
            SKEncodedImageFormat format;
            switch (Path.GetExtension(fileName).ToUpper(CultureInfo.InvariantCulture))
            {
                case ".PNG":
                    format = SKEncodedImageFormat.Png;
                    break;
                default:
                    format = SKEncodedImageFormat.Jpeg;
                    break;
            }

            return format;
        }
    }
}
