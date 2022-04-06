using Avalonia;
using Avalonia.Media;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Drawing
{
    public class LineBrush : Brush
    {
        public static readonly StyledProperty<Brush> BrushProperty =
            AvaloniaProperty.Register<LineBrush, Brush>(CommonInternalHelper.GetStyledPropertyName());

        public static readonly StyledProperty<LineStyle> LineStyleProperty =
            AvaloniaProperty.Register<LineBrush, LineStyle>(CommonInternalHelper.GetStyledPropertyName());

        static LineBrush()
        {
            AffectsRender<LineBrush>(BrushProperty, LineStyleProperty);
        }

        public Brush Brush
        {
            get => GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        public LineStyle LineStyle
        {
            get => GetValue(LineStyleProperty);
            set => SetValue(LineStyleProperty, value);
        }

        public override IBrush ToImmutable()
        {
            return new LineBrush
            {
                Brush = Brush,
                LineStyle = LineStyle
            };
        }
    }
}