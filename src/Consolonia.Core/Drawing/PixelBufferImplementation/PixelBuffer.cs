using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
            Span<Pixel> bufferSpan = _buffer.AsSpan();
            var backgroundPixel = new Pixel(new PixelBackground(Colors.Black));
            bufferSpan.Fill(backgroundPixel);
        }

        public ushort Width { get; }
        public ushort Height { get; }

        [JsonIgnore]
        public ref Pixel this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[i];
        }

        [JsonIgnore]
        public ref Pixel this[PixelBufferCoordinate point]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[ToIndex(point.X, point.Y)];
        }

        [JsonIgnore]
        public ref Pixel this[Point point]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[ToIndex((ushort)point.X, (ushort)point.Y)];
        }

        [JsonIgnore]
        public ref Pixel this[ushort x, ushort y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffer[ToIndex(x, y)];
        }

        [JsonIgnore] public int Length => _buffer.Length;

        [JsonIgnore] public Rect Size => new(0, 0, Width, Height);

        /// <summary>
        /// Gets a reference to the pixel at the specified coordinates for efficient in-place modification
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Pixel GetPixel(ushort x, ushort y)
        {
            return ref _buffer[ToIndex(x, y)];
        }

        /// <summary>
        /// Gets a reference to the pixel at the specified coordinate for efficient in-place modification
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Pixel GetPixel(PixelBufferCoordinate point)
        {
            return ref _buffer[ToIndex(point.X, point.Y)];
        }

        /// <summary>
        /// Gets a reference to the pixel at the specified linear index for efficient in-place modification
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Pixel GetPixel(int index)
        {
            return ref _buffer[index];
        }

        /// <summary>
        /// Gets a span over the entire buffer for efficient bulk operations
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<Pixel> AsSpan()
        {
            return _buffer.AsSpan();
        }

        /// <summary>
        /// Gets a span over a specific row for efficient row-wise operations
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<Pixel> GetRowSpan(ushort y)
        {
            int start = ToIndex(0, y);
            return _buffer.AsSpan(start, Width);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ToIndex(ushort x, ushort y)
        {
            return y * Width + x;
        }
    }
}