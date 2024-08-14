using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Reactive;
using Avalonia.Threading;
using Consolonia.Core.Drawing;
using Consolonia.Core.Helpers;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public class ConsoloniaTextPresenter : TextPresenter
    {
        private static readonly FieldInfo TickTimerField =
            typeof(TextPresenter).GetField("_caretTimer", BindingFlags.NonPublic | BindingFlags.Instance)!;

        static ConsoloniaTextPresenter()
        {
            CaretIndexProperty.Changed
                .SubscribeAction(args =>
                {
                    if (args.Sender is not TextPresenter textPresenter)
                        return;

                    // once avalonia moved the caret we then moving it additionally to scroll outside of the boundaries
                    
                    int caretIndex = args.NewValue.Value;

                    Dispatcher.UIThread.Post(
                        () =>
                        {
                            Rect hitTestTextPosition = textPresenter.TextLayout.HitTestTextPosition(caretIndex);
                            textPresenter.BringIntoView(new Rect(hitTestTextPosition.X, hitTestTextPosition.Y, 1, 1));
                        },
                        DispatcherPriority
                            .UiThreadRender /*Must be lower than DispatcherPriority.AfterRender which is used by TextPresenter*/);
                });

            CaretBrushProperty.Changed
                .Subscribe(
                    new AnonymousObserver<AvaloniaPropertyChangedEventArgs<IBrush>>(
                        args =>
                        {
                            if (args.NewValue.Value is not MoveConsoleCaretToPositionBrush)
                                throw new NotSupportedException();
                        }));
        }

        public ConsoloniaTextPresenter()
        {
            // we need to disable blinking caret, our terminal caret blinks itself once shown
            var caretTickTimer = (DispatcherTimer)TickTimerField.GetValue(this);
            caretTickTimer.Interval =
                TimeSpan.FromMilliseconds(int
                    .MaxValue); //see DispatcherTimer.Interval, since we can not disable it, setting it to longest interval possible
            caretTickTimer.Tick += (_, _) => throw new NotImplementedException("How to disable timer completely?");

            CaretBrush = new MoveConsoleCaretToPositionBrush();
        }

        protected override TextLayout CreateTextLayout()
        {
            // adding one more character space to accomodate the caret: https://github.com/AvaloniaUI/Avalonia/commit/bfae67dbdbe1d9058443065e425f71bdb855e547#r145302445

            //todo: check if optimizations possible here
            TextLayout textLayout = base.CreateTextLayout();

            {
                object metrics = typeof(TextLayout)
                    .GetField("_metrics", BindingFlags.Instance | BindingFlags.NonPublic)!
                    .GetValue(textLayout)!;

                FieldInfo widthIncludingTrailingWhitespaceField = metrics.GetType()
                    .GetField("WidthIncludingTrailingWhitespace", BindingFlags.Instance | BindingFlags.Public)!;
                double w = (double)widthIncludingTrailingWhitespaceField.GetValue(metrics)!;
                widthIncludingTrailingWhitespaceField.SetValue(metrics, w + 1);
            }

            foreach (TextLine textLayoutTextLine in textLayout.TextLines)
            {
                FieldInfo textLineMetricsField = textLayoutTextLine.GetType()
                    .GetField("_textLineMetrics", BindingFlags.Instance | BindingFlags.NonPublic)!;

                object textLineMetrics = textLineMetricsField.GetValue(textLayoutTextLine);

                PropertyInfo widthIncludingTrailingWhitespaceProperty = textLineMetrics.GetType()
                    .GetProperty("WidthIncludingTrailingWhitespace", BindingFlags.Instance | BindingFlags.Public);

                double w = (double)widthIncludingTrailingWhitespaceProperty.GetValue(textLineMetrics)!;
                widthIncludingTrailingWhitespaceProperty.SetValue(textLineMetrics, w + 1);


                textLineMetricsField.SetValue(textLayoutTextLine, textLineMetrics);
            }

            return textLayout;
        }
    }
}