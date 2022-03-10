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
        private bool _caretBlinking;

        private static readonly FieldInfo _tickTimerField =
            typeof(TextPresenter).GetField("_caretTimer", BindingFlags.NonPublic | BindingFlags.Instance)!;

        static ConsoloniaTextPresenter()
        {
            CaretBrushProperty.Changed.Subscribe(static args =>
            {
                if (args.NewValue.Value is not MoveConsoleCaretToPositionBrush consoleCaretBrush)
                    throw new NotSupportedException();
            });
        }

        public ConsoloniaTextPresenter()
        {
            var caretTickTimer = (DispatcherTimer)_tickTimerField.GetValue(this);
            caretTickTimer.Tag = this;

            CaretBrush = new MoveConsoleCaretToPositionBrush();
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
                .GetMethod(nameof(GetCaretPoints), BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(this, null);
        }

        public void CaptureTimerStart()
        {
            _caretBlinking = true;
        }

        public void CaptureTimerStop()
        {
            _caretBlinking = false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size measureOverride = base.MeasureOverride(availableSize);
            return measureOverride.WithWidth(measureOverride.Width + 1);
        }
    }
}