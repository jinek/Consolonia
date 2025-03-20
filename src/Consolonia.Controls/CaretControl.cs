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
            AvaloniaProperty.Register<CaretControl, CaretStyle>(nameof(CaretStyle));

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

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            CoerceValue(IsCaretShownProperty);
        }
    }
}