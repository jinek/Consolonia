using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    /// <summary>
    ///     A control that will draw a horizontal or a vertical line to separate content.
    ///     Orientation = Horizontal|Vertical
    ///     Brush = brush to use for the line.
    /// </summary>
    public class FastLineSeparator : Control
    {
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<FastLineSeparator, Orientation>(nameof(Orientation));

        public static readonly StyledProperty<IBrush> BrushProperty =
            AvaloniaProperty.Register<FastLineSeparator, IBrush>(nameof(Brush), Brushes.Black);


        public FastLineSeparator()
        {
            AffectsRender<FastLineSeparator>(OrientationProperty);
        }

        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public IBrush Brush
        {
            get => GetValue(BrushProperty);
            set => SetValue(BrushProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return Orientation == Orientation.Horizontal
                ? new Size(0, 1)
                : new Size(1, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return finalSize;
        }

        public override void Render(DrawingContext context)
        {
            var pen = new Pen(Brush);

            if (Orientation == Orientation.Horizontal)
            {
                // Draw a horizontal line across the control's width
                var startPoint = new Point(0, Bounds.Height / 2);
                var endPoint = new Point(Bounds.Width - 1, Bounds.Height / 2);
                context.DrawLine(pen, startPoint, endPoint);
            }
            else
            {
                // Draw a vertical line across the control's height
                var startPoint = new Point(Bounds.Width / 2, 0);
                var endPoint = new Point(Bounds.Width / 2, Bounds.Height - 1);
                context.DrawLine(pen, startPoint, endPoint);
            }
        }
    }
}