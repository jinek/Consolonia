using Avalonia;
using Avalonia.Media;

namespace Consolonia.Core.Drawing
{
    public class MoveConsoleCaretToPositionBrush : IImmutableBrush
    {
        //todo: Search for B75ABC91-2CDD-4557-9201-16AC483C8D7B
        public double Opacity => 1;
        public ITransform Transform { get; }
        public RelativePoint TransformOrigin { get; }
    }
}