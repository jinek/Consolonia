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
        private static int _counter;
        private Pixel[,] _buffer;
        private string _id;

        public PixelBuffer(PixelBufferSize size)
            : this(new PixelBufferCoordinate(0,0), size)
        {
        }

        public PixelBuffer(PixelBufferCoordinate point, PixelBufferSize size)
        {
            _id = _counter++.ToString();
            this.Position = point;
            SetBufferSize(size);
        }

        public void SetBufferSize(PixelBufferSize size)
        {
            var id = int.Parse(_id);
            var colors = new Color[] { Colors.Black, Colors.White, Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Cyan, Colors.Magenta };

            if (size.Width != Size.Width && size.Height != Size.Height)
            {
                this.Size = size;
                _buffer = new Pixel[size.Width, size.Height];

                // initialize the buffer with space so it draws any background color
                // blended into it.
                for (ushort y = 0; y < size.Height; y++)
                    for (ushort x = 0; x < size.Width; x++)
                    {
                        // _buffer[x, y] = new Pixel(new PixelBackground(Colors.Black));
                        _buffer[x, y] = new Pixel(new PixelForeground(new SimpleSymbol(_id), Colors.White), new PixelBackground(colors[id % colors.Length]));
                    }
            }
        }

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

        [JsonIgnore] public PixelBufferCoordinate Position { get; set; }

        [JsonIgnore] public PixelBufferSize Size { get; private set; }

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
            for (ushort j = 0; j < Size.Height; j++)
                for (ushort i = 0; i < Size.Width; i++)
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
        public void BitBlt(int left, int top, PixelBuffer targetPixelBuffer)
        {
            lock (targetPixelBuffer)
            {
                for (ushort x = 0; x < Size.Width; x++)
                {
                    for (ushort y = 0; y < Size.Height; y++)
                    {
                        var targetX = (ushort)(x + left);
                        var targetY = (ushort)(y + top);
                        if (targetX >= 0 && targetX < targetPixelBuffer.Size.Width &&
                            targetY >= 0 && targetY < targetPixelBuffer.Size.Height)
                        {
                            targetPixelBuffer.Set(new PixelBufferCoordinate(targetX, targetY),
                                    pixel => pixel.Blend(this[x, y]));
                        }
                    }
                }
            }
        }

        private (ushort x, ushort y) ToXY(int i)
        {
            return ((ushort x, ushort y))(i % Size.Width, i / Size.Width);
        }
    }
}