using System;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure
{
    public interface IConsole
    {
        ConsoleSize Size { get; }
        bool CaretVisible { get; set; }
        void MoveCaretForControl(ConsolePosition? position, int size, object ownerControl);
        void AddCaretFor(object control);
        void RemoveCaretFor(object control);
        IDisposable StoreCaret();
        void SetCaretPosition(ConsolePosition position);
        ConsolePosition GetCaretPosition();

        void Print(ConsolePosition position, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            char character);

        event Action Resized;
        event Action<Key, char, RawInputModifiers> KeyPress;
    }
}