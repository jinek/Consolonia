using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Consolonia.Controls
{
    public class CaretControl : ContentControl
    {
        public static readonly StyledProperty<bool> IsCaretShownProperty =
            AvaloniaProperty.Register<CaretControl, bool>(nameof(IsCaretShown));

        public static readonly StyledProperty<CaretStyle> CaretStyleProperty =
            AvaloniaProperty.Register<CaretControl, CaretStyle>(nameof(CaretStyle), CaretStyle.BlinkingBar);

        public static readonly StyledProperty<Thickness> CaretMarginProperty =
            AvaloniaProperty.Register<CaretControl, Thickness>(ControlUtils.GetStyledPropertyName());

        public bool IsCaretShown
        {
            get => GetValue(IsCaretShownProperty);
            set => SetValue(IsCaretShownProperty, value);
        }

        public CaretStyle CaretStyle
        {
            get => GetValue(CaretStyleProperty);
            set => SetValue(CaretStyleProperty, value);
        }

        public Thickness CaretMargin
        {
            get => GetValue(CaretMarginProperty);
            set => SetValue(CaretMarginProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            CoerceValue(IsCaretShownProperty);
        }
    }
}