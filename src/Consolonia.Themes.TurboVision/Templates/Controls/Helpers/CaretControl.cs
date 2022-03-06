using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public class CaretControl : ContentControl
    {
        public static readonly StyledProperty<bool> IsCaretShownProperty =
            AvaloniaProperty.Register<CaretControl, bool>(nameof(IsCaretShown));


        public bool IsCaretShown
        {
            get => GetValue(IsCaretShownProperty);
            set => SetValue(IsCaretShownProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            CoerceValue(IsCaretShownProperty);
        }
    }
}