using System;
using Avalonia.Input;
using Consolonia.Core.Drawing.PixelBufferImplementation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Consolonia.Core.Infrastructure
{
    public interface IConsole : IDisposable
    {
        PixelBufferSize Size { get; }
        bool CaretVisible { get; set; }

        void SetCaretPosition(PixelBufferCoordinate bufferPoint);
        PixelBufferCoordinate GetCaretPosition();

        void Print(PixelBufferCoordinate bufferPoint, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            string str);

        event Action Resized;
        event Action<Key, char, RawInputModifiers> KeyPress;
    }
}