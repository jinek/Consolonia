using Avalonia;
using Avalonia.Media;

namespace Consolonia.Core.Drawing
{
    public class MoveConsoleCaretToPositionBrush : IImmutableBrush
    {
        //todo: Search for B75ABC91-2CDD-4557-9201-16AC483C8D7B
        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;

        /// <summary>
        /// style of curosr
        /// </summary>
        public CursorStyle Style { get; set; } = CursorStyle.BlinkingBar;
    }
}