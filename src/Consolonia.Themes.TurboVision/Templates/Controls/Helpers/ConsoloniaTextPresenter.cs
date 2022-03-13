using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using Consolonia.Core.Drawing;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public class ConsoloniaTextPresenter : TextPresenter, ICaptureTimerStartStop
    {
        private static readonly FieldInfo TickTimerField =
            typeof(TextPresenter).GetField("_caretTimer", BindingFlags.NonPublic | BindingFlags.Instance)!;

        private bool _caretBlinking;

        static ConsoloniaTextPresenter()
        {
            CaretBrushProperty.Changed.Subscribe(static args =>
            {
                if (args.NewValue.Value is not MoveConsoleCaretToPositionBrush)
                    throw new NotSupportedException();
            });
        }

        public ConsoloniaTextPresenter()
        {
            var caretTickTimer = (DispatcherTimer)TickTimerField.GetValue(this);
            caretTickTimer!.Tag = this;

            CaretBrush = new MoveConsoleCaretToPositionBrush();
        }

        public void CaptureTimerStart()
        {
            _caretBlinking = true;
        }

        public void CaptureTimerStop()
        {
            _caretBlinking = false;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            if (SelectionStart == SelectionEnd || !_caretBlinking) return;
            (Point p1, Point p2) = GetCaretPoints();
            context.DrawLine(
                new Pen(CaretBrush),
                p1, p2);
        }

        private (Point, Point) GetCaretPoints()
        {
            return ((Point, Point))typeof(TextPresenter)
                .GetMethod(nameof(GetCaretPoints), BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(this, null)!;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size measureOverride = base.MeasureOverride(availableSize);
            return measureOverride.WithWidth(measureOverride.Width + 1);
        }
    }
}