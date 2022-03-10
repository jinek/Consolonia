using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelBuffer : IEnumerable<Pixel>
    {
        public void Set(PixelBufferCoordinate point, Func<Pixel, Pixel> changeAction)
        {
            Set<object>(point, (pixel, _) => changeAction(pixel), null);
        }

        public void Set<TUserObject>(PixelBufferCoordinate point, Func<Pixel, TUserObject, Pixel> changeAction, TUserObject userObject)
        {
            this[point] = changeAction(this[point], userObject);
        }

        public void Foreach(Func<PixelBufferCoordinate, Pixel, Pixel> replaceAction)
        {
            ForeachReadonly((point, oldPixel) =>
            {
                var newPixel = replaceAction(point, oldPixel);
                this[point] = newPixel;
            });
        }

        public void ForeachReadonly(Action<PixelBufferCoordinate, Pixel> action)
        {
            for (ushort i = 0; i < Width; i++)
                for (ushort j = 0; j < Height; j++)
                {
                    Pixel pixel = this[(PixelBufferCoordinate)(i, j)];
                    action(new PixelBufferCoordinate(i, j), pixel);
                }
        }

        public ushort Width { get; }
        public ushort Height { get; }

        public PixelBuffer(ushort width, ushort height)
        {
            Width = width;
            Height = height;
            _buffer = new Pixel[width, height];
        }

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

        public Pixel this[PixelBufferCoordinate point]
        {
            get => _buffer[point.X, point.Y];
            set => _buffer[point.X, point.Y] = value;
        }

        private (ushort x, ushort y) ToXY(int i)
        {
            return ((ushort x, ushort y))(i % Width, i / Width);
        }

        private readonly Pixel[,] _buffer;

        public IEnumerator<Pixel> GetEnumerator()
        {
            return _buffer.OfType<Pixel>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Length => _buffer.Length;
        public Rect Size => new(0, 0, Width, Height);
    }
}