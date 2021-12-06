using Avalonia;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    public interface IConsoleCaretBrush
    {
        void SetOwnerControl(IAvaloniaObject owner);
        void MoveCaret(ConsolePosition? position, int size);
    }
}