using Avalonia;

namespace Consolonia.Core.Drawing
{
    public interface IConsoleCaretBrush
    {
        void SetOwnerControl(IAvaloniaObject owner);
        void MoveCaret(Point? position, int size);
    }
}