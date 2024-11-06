using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public class PixelBuffer : IEnumerable<Pixel>
    {
        private readonly Pixel[,] _buffer;
        private PixelBufferCoordinate _caretPosition;

        public PixelBuffer(ushort width, ushort height)
        {
            Width = width;
            Height = height;
            _buffer = new Pixel[width, height];
            _caretPosition = new PixelBufferCoordinate(0, 0);
        }

        public ushort Width { get; }
        public ushort Height { get; }

        // ReSharper disable once UnusedMember.Global
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
            // ReSharper disable once MemberCanBePrivate.Global
            set => _buffer[point.X, point.Y] = value;
        }

        public int Length => _buffer.Length;
        public Rect Size => new(0, 0, Width, Height);

        public IEnumerator<Pixel> GetEnumerator()
        {
            return _buffer.OfType<Pixel>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Set(PixelBufferCoordinate point, Func<Pixel, Pixel> changeAction)
        {
            Set<object>(point, (pixel, _) => changeAction(pixel), null);
        }

        public void Set<TUserObject>(PixelBufferCoordinate point, Func<Pixel, TUserObject, Pixel> changeAction,
            TUserObject userObject)
        {
            this[point] = changeAction(this[point], userObject);
        }

        /// <summary>
        /// Clears old pixel caret position and sets new caret position
        /// </summary>
        /// <param name="point"></param>
        public void SetCaretPosition(PixelBufferCoordinate point)
        {
            var oldCaretPixel = _buffer[_caretPosition.X, _caretPosition.Y];
            _buffer[_caretPosition.X, _caretPosition.Y] = new Pixel(oldCaretPixel.Foreground, oldCaretPixel.Background, isCaret: false);

            _caretPosition = point;
            var newCaretPixel = _buffer[_caretPosition.X, _caretPosition.Y];
            _buffer[_caretPosition.X, _caretPosition.Y] = new Pixel(newCaretPixel.Foreground, newCaretPixel.Background, isCaret: true);
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