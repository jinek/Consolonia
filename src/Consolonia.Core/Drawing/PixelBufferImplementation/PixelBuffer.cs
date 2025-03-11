using System;
using Avalonia;
using Avalonia.Media;
using Consolonia.Controls;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [JsonConverter(typeof(PixelBufferConverter))]
    public class PixelBuffer
    {
        private Pixel[,] _buffer;

        public PixelBuffer(PixelBufferSize size)
            : this(size.Width, size.Height)
        {
        }

        public PixelBuffer(ushort width, ushort height)
        {
            SetBufferSize(width, height);
        }

        public void SetBufferSize(ushort width, ushort height)
        {
            Width = width;
            Height = height;
            _buffer = new Pixel[width, height];

            // initialize the buffer with space so it draws any background color
            // blended into it.
            for (ushort y = 0; y < height; y++)
                for (ushort x = 0; x < width; x++)
                    _buffer[x, y] = new Pixel(new PixelBackground(Colors.Black));
        }

        public ushort Width { get; private set; }
        public ushort Height { get; private set; }

        public CaretStyle CaretStyle { get; set; } = CaretStyle.BlinkingBar;

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

        [JsonIgnore] public int Length => _buffer.Length;

        [JsonIgnore] public Rect Size => new(0, 0, Width, Height);


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

        /// <summary>
        /// Blend this pixel buffer into another pixelbuffer at a given location
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetPixelBuffer"></param>
        public void BitBlt(int x, int y, PixelPoint position, PixelBuffer targetPixelBuffer)
            => BitBlt(new PixelPoint(x, y), targetPixelBuffer);

        /// <summary>
        /// Blend this pixel buffer into another pixelbuffer at a given location
        /// </summary>
        /// <param name="position"></param>
        /// <param name="targetPixelBuffer"></param>
        public virtual void BitBlt(PixelPoint position, PixelBuffer targetPixelBuffer)
        {
            lock (targetPixelBuffer)
            {
                for (ushort x = 0; x < Size.Width; x++)
                {
                    for (ushort y = 0; y < Size.Height; y++)
                    {
                        var targetX = (ushort)(x + position.X);
                        var targetY = (ushort)(y + position.Y);
                        if (targetX >= 0 && targetX < targetPixelBuffer.Width &&
                            targetY >= 0 && targetY < targetPixelBuffer.Height)
                        {
                            targetPixelBuffer.Set((PixelBufferCoordinate)new PixelBufferCoordinate(targetX, targetY),
                                    pixel => pixel.Blend(this[x, y]));
                        }
                    }
                }
            }
        }

        private (ushort x, ushort y) ToXY(int i)
        {
            return ((ushort x, ushort y))(i % Width, i / Width);
        }
    }
}