using System;
using Avalonia.Input;
using Consolonia.Core.Drawing.PixelBuffer;

namespace Consolonia.Core.Infrastructure
{
    public interface IConsole
    {
        PixelBufferSize Size { get; }
        bool CaretVisible { get; set; }

        void SetCaretPosition(PixelBufferCoordinate bufferPoint);
        PixelBufferCoordinate GetCaretPosition();

        void Print(PixelBufferCoordinate bufferPoint, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            char character);

        event Action Resized;
        event Action<Key, char, RawInputModifiers> KeyPress;
    }
}