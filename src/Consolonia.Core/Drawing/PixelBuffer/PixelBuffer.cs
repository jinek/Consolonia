using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace Consolonia.Core.Drawing.PixelBuffer
{
    public class PixelBuffer : IEnumerable<Pixel>
    {
        public void Set(PixelBufferCoordinate point, Func<Pixel, Pixel> changeAction)
        {
            Set<object>(point, (pixel, _) => changeAction(pixel), null);
        }
    
        public void Set<TUserObject>(PixelBufferCoordinate point, Func<Pixel,TUserObject, Pixel> changeAction, TUserObject userObject)
        {
            this[point] = changeAction(this[point],userObject);
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
            for (ushort j = 0; j < Height; j++)
            for (ushort i = 0; i < Width; i++)
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
            Buffer = new Pixel[width, height];
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
            get => Buffer[point.X, point.Y];
            set => Buffer[point.X, point.Y] = value;
        }

        private (ushort x, ushort y) ToXY(int i)
        {
            return ((ushort x, ushort y))(i % Width, i / Width);
        }

        public readonly Pixel[,] Buffer;

        public IEnumerator<Pixel> GetEnumerator()
        {
            return Buffer.OfType<Pixel>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Length => Buffer.Length;
        public Rect Size => new(0, 0, Width, Height);
    }
}