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
        //todo: we don't really implement immutable brush
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

        //todo: how did it work without following 3 items? How should it work now, check avalonia. Search for B75ABC91-2CDD-4557-9201-16AC483C8D7B
        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;
    }
}