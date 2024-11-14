using System;
using Avalonia;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [JsonConverter(typeof(PixelBufferConverter))]
    public class PixelBuffer
    {
        private readonly Pixel[,] _buffer;

        public PixelBuffer(ushort width, ushort height)
        {
            Width = width;
            Height = height;
            _buffer = new Pixel[width, height];
            for (ushort y = 0; y < height; y++)
                for (ushort x = 0; x < width; x++)
                    _buffer[x, y] = new Pixel();
        }

        public ushort Width { get; }
        public ushort Height { get; }

        // ReSharper disable once UnusedMember.Global
        [JsonIgnore]
        public Pixel this[int i]
        {
            get
            {
                (ushort x, ushort y) = ToXY(i);
                return this[(PixelBufferCoordinate)(x, y)];
            }
            set
            {
                (ushort x, ushort y) = ToXY(i);
                this[(PixelBufferCoordinate)(x, y)] = value;
            }
        }

        [JsonIgnore]
        public Pixel this[PixelBufferCoordinate point]
        {
            get => _buffer[point.X, point.Y];
            // ReSharper disable once MemberCanBePrivate.Global
            set => _buffer[point.X, point.Y] = value;
        }

        [JsonIgnore]
        public Pixel this[ushort x, ushort y]
        {
            get => _buffer[x, y];
            // ReSharper disable once MemberCanBePrivate.Global
            set => _buffer[x, y] = value;
        }

        [JsonIgnore]
        public int Length => _buffer.Length;

        [JsonIgnore]
        public Rect Size => new(0, 0, Width, Height);


        public void Set(PixelBufferCoordinate point, Func<Pixel, Pixel> changeAction)
        {
            Set<object>(point, (pixel, _) => changeAction(pixel), null);
        }

        public void Set<TUserObject>(PixelBufferCoordinate point, Func<Pixel, TUserObject, Pixel> changeAction,
            TUserObject userObject)
        {
            this[point] = changeAction(this[point], userObject);
        }

        public void Foreach(Func<PixelBufferCoordinate, Pixel, Pixel> replaceAction)
        {
            ForeachReadonly((point, oldPixel) =>
            {
                Pixel newPixel = replaceAction(point, oldPixel);
                this[point] = newPixel;
            });
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void ForeachReadonly(Action<PixelBufferCoordinate, Pixel> action)
        {
            for (ushort j = 0; j < Height; j++)
                for (ushort i = 0; i < Width; i++)
                {
                    Pixel pixel = this[(PixelBufferCoordinate)(i, j)];
                    action(new PixelBufferCoordinate(i, j), pixel);
                }
        }

        private (ushort x, ushort y) ToXY(int i)
        {
            return ((ushort x, ushort y))(i % Width, i / Width);
        }
    }
}