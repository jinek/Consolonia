using System;
using Avalonia;

namespace Consolonia.Core.Infrastructure
{
    public interface IConsole
    {
        void MoveCaretForControl(Point? position, int size, object ownerControl);
        void AddCaretFor(object control);
        void RemoveCaretFor(object control);
        IDisposable StoreCaret();
        void SetCaretPosition((ushort x, ushort y) position);
        (ushort x, ushort y) GetCaretPosition();
        void Print(ushort x, ushort y, ConsoleColor backgroundColor, ConsoleColor foregroundColor, char character);
        event Action Resized;
        (ushort width, ushort height) Size { get; }
    }
}