using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Drawing
{
    public class LineBrush : Animatable, IImmutableBrush
    {
        //todo: we don't really implement immutable brush
        public static readonly StyledProperty<IBrush> BrushProperty =
            AvaloniaProperty.Register<LineBrush, IBrush>(CommonInternalHelper.GetStyledPropertyName());

        public static readonly StyledProperty<LineStyle> LineStyleProperty =
            AvaloniaProperty.Register<LineBrush, LineStyle>(CommonInternalHelper.GetStyledPropertyName());

        static LineBrush()
        {
        }

        public IBrush Brush
        {
            get => GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public LineStyle LineStyle
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