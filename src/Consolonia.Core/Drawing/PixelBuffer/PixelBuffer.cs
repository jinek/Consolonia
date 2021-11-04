using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace Consolonia.Core.Drawing.PixelBuffer
{
    public class PixelBuffer : IEnumerable<Pixel>
    {
        public void Set(int x, int y, Func<Pixel, Pixel> changeAction)
        {
            this[x, y] = changeAction(this[x, y]);
        }

        public void Foreach(Func<int, int, Pixel, Pixel> replaceAction)
        {
            ForeachReadonly((x, y, oldPixel) =>
            {
                var newPixel = replaceAction(x, y, oldPixel);
                this[x, y] = newPixel;
            });
        }

        public void ForeachReadonly(Action<int, int, Pixel> action)
        {
            for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
            {
                Pixel pixel = this[i, j];
                action(i, j, pixel);
            }
        }

        public short Width { get; }
        public short Height { get; }

        public PixelBuffer(short width, short height)
        {
            Width = width;
            Height = height;
            Buffer = new Pixel[width, height];
            /*Foreach((_, _, _) => new Pixel
            {
                Foreground = new PixelForeground
                {
                    Symbol = new SimpleSymbol()
                }
            });*/
        }

        public Pixel this[int i]
        {
            get
            {
                (short x, short y) = ToXY(i);
                return this[x, y];
            }
            set
            {
                (short x, short y) = ToXY(i);
                this[x, y] = value;
            }
        }

        public Pixel this[int x, int y]
        {
            get => Buffer[x, y];
            set => Buffer[x, y] = value;
        }

        private (short x, short y) ToXY(int i)
        {
            return ((short x, short y))(i % Width, i / Width);
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