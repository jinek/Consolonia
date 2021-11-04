using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    public class CaretControl : ContentControl
    {
        static CaretControl()
        {
            IsCaretShownProperty.Changed.Subscribe(static args =>
            {
                var caretControl = (CaretControl)args.Sender!;
                ((MoveConsoleCaretToPositionBrush)caretControl._line.Stroke).SetOwnerControl(caretControl);
                var _console = AvaloniaLocator.Current.GetService<IConsole>();
                if (args.NewValue.Value)
                    _console.AddCaretFor(args.Sender);
                else _console.RemoveCaretFor(args.Sender);
            });
        }

        public static readonly StyledProperty<bool> IsCaretShownProperty =
            AvaloniaProperty.Register<CaretControl, bool>(nameof(IsCaretShown));

        private Line _line;

        public bool IsCaretShown
        {
            get => GetValue(IsCaretShownProperty);
            set => SetValue(IsCaretShownProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _line = e.NameScope.Find<Line>("PART_CaretLine");
        }
    }
}