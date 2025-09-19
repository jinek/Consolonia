using System;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia;
using Avalonia.Media;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [JsonConverter(typeof(PixelBufferConverter))]
    public class PixelBuffer
    {
        private readonly Pixel[] _buffer;

        public PixelBuffer(PixelBufferSize size)
            : this(size.Width, size.Height)
        {
        }

        public PixelBuffer(ushort width, ushort height)
        {
            Width = width;
            Height = height;
            _buffer = new Pixel[width * height];

            // initialize the buffer with space so it draws any background color
            // blended into it.
            for (int i = 0; i < _buffer.Length; i++)
                _buffer[i] = new Pixel(new PixelBackground(Colors.Black));
        }

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
            get => _buffer[point.X + point.Y * Width];
            // ReSharper disable once MemberCanBePrivate.Global
            set => _buffer[point.X + point.Y * Width] = value;
        }

        [JsonIgnore]
        public Pixel this[ushort x, ushort y]
        {
            get => _buffer[x + y * Width];
            // ReSharper disable once MemberCanBePrivate.Global
            set => _buffer[x + y * Width] = value;
        }

        [JsonIgnore]
        public Pixel this[Point point]
        {
            get => this[(PixelBufferCoordinate)point];
            set => this[(PixelBufferCoordinate)point] = value;
        }

        [JsonIgnore] public int Length => _buffer.Length;


        [JsonIgnore] public Rect Size => new(0, 0, Width, Height);

        public Span<Pixel> GetRowSpan(ushort y)
        {
            return new Span<Pixel>(_buffer, y * Width, Width);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (ushort x, ushort y) ToXY(int i)
        {
            return ((ushort x, ushort y))(i % Width, i / Width);
        }

        public string PrintBuffer()
        {
            var stringBuilder = new StringBuilder();

            for (ushort j = 0; j < Height; j++)
            {
                for (ushort i = 0; i < Width; i++)
                {
                    if (i == Width - 1 && j == Height - 1)
                        break;
                    Pixel pixel = this[new PixelBufferCoordinate(i, j)];
                    string text = pixel.IsCaret() ? "Ꮖ" : pixel.Foreground.Symbol.GetText();

                    //todo: check why cursor is not drawing
                    stringBuilder.Append(text);
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

#pragma warning disable CA1051 // Do not declare visible instance fields
        public readonly ushort Width;
        public readonly ushort Height;
#pragma warning restore CA1051 // Do not declare visible instance fields
    }
}