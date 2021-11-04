using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    public class MoveConsoleCaretToPositionBrush : IBrush, IConsoleCaretBrush
    {
        private IAvaloniaObject _owner;
        private readonly IConsole _console = AvaloniaLocator.Current.GetService<IConsole>()!;
        public double Opacity => 1;

        public void SetOwnerControl(IAvaloniaObject owner)
        {
            _owner = owner;
        }

        public void MoveCaret(Point position, int size)
        {
            _console.MoveCaretForControl(position, size, _owner);
        }
    }
}