using Avalonia;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    public interface IConsoleCaretBrush
    {
        void SetOwnerControl(IAvaloniaObject owner);
        void MoveCaret(Point position, int size);
    }
}