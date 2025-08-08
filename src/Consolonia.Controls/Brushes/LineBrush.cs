using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

// ReSharper disable CheckNamespace
namespace Consolonia.Controls.Brushes
{
    /// <summary>
    ///     This brush will draw using a LineStyle
    /// </summary>
    public class LineBrush : Animatable, IImmutableBrush
    {
        public static readonly StyledProperty<IBrush> BrushProperty =
            AvaloniaProperty.Register<LineBrush, IBrush>(
                ControlUtils.GetStyledPropertyName() /*todo: re-use this method everywhere*/);

        public static readonly StyledProperty<LineStyles> LineStyleProperty =
            AvaloniaProperty.Register<LineBrush, LineStyles>(ControlUtils.GetStyledPropertyName());

        public IBrush Brush
        {
            get => GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public LineStyles LineStyle
        {
            get => GetValue(LineStyleProperty);
            set => SetValue(LineStyleProperty, value);
        }

        /// <inheritdoc />
        public double Opacity => Brush?.Opacity ?? 1;

        /// <inheritdoc />
        public ITransform Transform => Brush?.Transform;

        /// <inheritdoc />
        public RelativePoint TransformOrigin => Brush?.TransformOrigin ?? RelativePoint.TopLeft;

        /// <summary>
        ///     Creates an immutable snapshot of this brush.
        /// </summary>
        public IImmutableBrush ToImmutable()
        {
            return new ImmutableLineBrush(Brush?.ToImmutable(),
                new LineStyles(LineStyle.Left, LineStyle.Top, LineStyle.Right, LineStyle.Bottom));
        }

        private sealed class ImmutableLineBrush : IImmutableBrush
        {
            public ImmutableLineBrush(IImmutableBrush brush, LineStyles lineStyle)
            {
                Brush = brush;
                LineStyle = lineStyle;
            }

            public IImmutableBrush Brush { get; }
            public LineStyles LineStyle { get; }

            public double Opacity => Brush?.Opacity ?? 1;
            public ITransform Transform => Brush?.Transform;
            public RelativePoint TransformOrigin => Brush?.TransformOrigin ?? RelativePoint.TopLeft;
        }
    }
}
