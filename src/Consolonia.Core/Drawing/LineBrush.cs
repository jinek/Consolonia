using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Drawing
{
    public class LineBrush : Animatable, IBrush
    {
        public static readonly StyledProperty<FourBitColorBrush> BrushProperty =
            AvaloniaProperty.Register<LineBrush, FourBitColorBrush>(CommonInternalHelper.GetStyledPropertyName());

        public static readonly StyledProperty<LineStyle> LineStyleProperty =
            AvaloniaProperty.Register<LineBrush, LineStyle>(CommonInternalHelper.GetStyledPropertyName());

        static LineBrush()
        {
        }

        public FourBitColorBrush Brush
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
        public double Opacity { get; }
        public ITransform Transform { get; }
        public RelativePoint TransformOrigin { get; }
    }
}