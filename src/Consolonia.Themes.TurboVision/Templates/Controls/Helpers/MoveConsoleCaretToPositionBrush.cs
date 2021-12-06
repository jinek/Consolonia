using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
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

        public void MoveCaret(ConsolePosition? position, int size)
        {
            _console.MoveCaretForControl(position, size, _owner);
        }
    }
}