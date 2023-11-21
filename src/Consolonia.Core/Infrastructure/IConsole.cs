using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Consolonia.Core.Drawing.PixelBufferImplementation;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace Consolonia.Core.Infrastructure
{
    public interface IConsole : IDisposable
    {
        PixelBufferSize Size { get; }
        bool CaretVisible { get; set; }
        void SetTitle(string title);

        void SetCaretPosition(PixelBufferCoordinate bufferPoint);
        PixelBufferCoordinate GetCaretPosition();

        void Print(PixelBufferCoordinate bufferPoint, ConsoleColor backgroundColor, ConsoleColor foregroundColor,
            string str);


        event Action Resized;
        event Action<Key, char, RawInputModifiers, bool, ulong> KeyEvent;
        event Action<RawPointerEventType, Point, Vector?, RawInputModifiers> MouseEvent;

        event Action<bool> FocusEvent;
        void PauseIO(Task task);
        void ClearOutput();
    }
}