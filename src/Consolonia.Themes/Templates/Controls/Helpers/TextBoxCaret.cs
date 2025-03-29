using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Avalonia.Reactive;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    public class TextBoxCaret : Control
    {
        public static readonly StyledProperty<bool> ActiveProperty =
            AvaloniaProperty.Register<TextBoxCaret, bool>(nameof(Active));

        public static readonly StyledProperty<Point> PositionProperty =
            AvaloniaProperty.Register<TextBoxCaret, Point>(nameof(Position));

        public static readonly StyledProperty<IBrush> CaretBrushProperty =
            TextBox.CaretBrushProperty.AddOwner<TextBoxCaret>();

        public TextBoxCaret()
        {
            PositionProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<Point>>(args =>
            {
                ((TextBoxCaret)args.Sender).InvalidateVisual();
            }));

            ActiveProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<bool>>(args =>
            {
                ((TextBoxCaret)args.Sender).InvalidateVisual();
            }));

            IsHitTestVisible = false;
            IsTabStop = false;
            Focusable = false;
        }

        public bool Active
        {
            get => GetValue(ActiveProperty);
            set => SetValue(ActiveProperty, value);
        }

        public Point Position
        {
            get => GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public IBrush CaretBrush
        {
            get => GetValue(CaretBrushProperty);
            set => SetValue(CaretBrushProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            if (!Active)
                return;

            context.DrawLine(new ImmutablePen(CaretBrush.ToImmutable()), Position,
                Position.WithY(Position.Y + 1));
        }
    }
}