using Avalonia;
using Avalonia.Media;

namespace Consolonia.Controls.Brushes
{
    /// <summary>
    ///     This brush will move the console caret to the specified position it is drawn into.
    /// </summary>
    public class MoveConsoleCaretToPositionBrush : AvaloniaObject, IImmutableBrush
    {
        public static readonly StyledProperty<CaretStyle> CaretStyleProperty =
            AvaloniaProperty.Register<MoveConsoleCaretToPositionBrush, CaretStyle>(nameof(CaretStyle),
                CaretStyle.BlinkingBar);

        /// <summary>
        ///     style of caret
        /// </summary>
        public CaretStyle CaretStyle
        {
            get => GetValue(CaretStyleProperty);
            set => SetValue(CaretStyleProperty, value);
        }

        // TODO B75ABC91-2CDD-4557-9201-16AC483C8D7B implement drawing with opacity and transform
        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;

        /// <summary>
        ///     Creates an immutable snapshot of this brush.
        /// </summary>
        public IImmutableBrush ToImmutable()
        {
            return new ImmutableMoveConsoleCaretToPositionBrush(CaretStyle);
        }

        private sealed class ImmutableMoveConsoleCaretToPositionBrush : IImmutableBrush
        {
            public ImmutableMoveConsoleCaretToPositionBrush(CaretStyle caretStyle)
            {
                CaretStyle = caretStyle;
            }

            public CaretStyle CaretStyle { get; }

            public double Opacity => 1;
            public ITransform Transform => null;
            public RelativePoint TransformOrigin => RelativePoint.TopLeft;
        }
    }
}