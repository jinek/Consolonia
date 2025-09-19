using System;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [JsonConverter(typeof(PixelBufferConverter))]
    public class PixelBuffer
    {

        public PixelBuffer(PixelBufferSize size)
            : this(size.Width, size.Height)
        {
        }

        public PixelBuffer(ushort width, ushort height)
        {
            Width = width;
            Height = height;
            Pixels = new Pixel[width, height];

            // initialize the buffer with space so it draws any background color
            // blended into it.
            for (ushort y = 0; y < height; y++)
            for (ushort x = 0; x < width; x++)
                Pixels[x, y] = new Pixel(new PixelBackground(Colors.Black));
        }

#pragma warning disable CA1051 // Do not declare visible instance fields
        [JsonProperty]
        public readonly ushort Width;
        [JsonProperty]
        public readonly ushort Height;

        [JsonProperty]
        public readonly Pixel[,] Pixels;
#pragma warning restore CA1051 // Do not declare visible instance fields

        [JsonIgnore] public int Length => Pixels.Length;

        [JsonIgnore] public Rect Size => new(0, 0, Width, Height);


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
                    Pixel pixel = this.Pixels[i, j];
                    string text = pixel.IsCaret() ? "Ꮖ" : pixel.Foreground.Symbol.GetText();

                    //todo: check why cursor is not drawing
                    stringBuilder.Append(text);
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}