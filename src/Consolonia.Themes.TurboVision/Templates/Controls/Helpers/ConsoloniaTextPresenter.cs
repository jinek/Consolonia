using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Threading;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public class ConsoloniaTextPresenter : TextPresenter
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly StyledProperty<Point> CaretPositionProperty =
            AvaloniaProperty.Register<ConsoloniaTextPresenter, Point>(nameof(CaretPosition));

        private static readonly FieldInfo TickTimerField =
            typeof(TextPresenter).GetField("_caretTimer", BindingFlags.NonPublic | BindingFlags.Instance)!;

        static ConsoloniaTextPresenter()
        {
            SelectionEndProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<int>>(args =>
            {
                if (args.Sender is not ConsoloniaTextPresenter textPresenter)
                    return;

                textPresenter.UpdateCaretPosition(null);
            }));

            CaretIndexProperty.Changed
                .SubscribeAction(args =>
                {
                    if (args.Sender is not ConsoloniaTextPresenter textPresenter)
                        return;

                    // once avalonia moved the caret we then moving it additionally to scroll outside the boundaries

                    int caretIndex = args.NewValue.Value;

                    Rect hitTestTextPosition = textPresenter.UpdateCaretPosition(caretIndex);

                    Dispatcher.UIThread.Post(
                        () =>
                        {
                            textPresenter.BringIntoView(new Rect(hitTestTextPosition.X, hitTestTextPosition.Y, 1, 1));
                        },
                        DispatcherPriority
                            .UiThreadRender /*Must be lower than DispatcherPriority.AfterRender which is used by TextPresenter*/);
                });

            CaretBrushProperty.Changed
                .Subscribe(
                    new AnonymousObserver<AvaloniaPropertyChangedEventArgs<IBrush>>(
                        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local //todo: what does this mean?
                        args =>
                        {
                            if (args.NewValue.Value is not FourBitColorBrush
                                {
                                    Color: ConsoleColor.Black, Mode: PixelBackgroundMode.Transparent
                                })
                                throw new NotSupportedException();
                        }));
        }

        public ConsoloniaTextPresenter()
        {
            // we need to disable blinking caret, our terminal caret blinks itself once shown
            var caretTickTimer = (DispatcherTimer)TickTimerField.GetValue(this);
            caretTickTimer!.Interval =
                TimeSpan.FromMilliseconds(int
                    .MaxValue); //see DispatcherTimer.Interval, since we can not disable it, setting it to the longest interval possible
            caretTickTimer!.Tick += (_, _) => throw new NotImplementedException("How to disable timer completely?");

            CaretBrush =
                new FourBitColorBrush(ConsoleColor.Black, PixelBackgroundMode.Transparent); // we want to draw own caret
        }

        public Point CaretPosition
        {
            get => GetValue(CaretPositionProperty);
            private set => SetValue(CaretPositionProperty, value);
        }

        private Rect UpdateCaretPosition(int? caretIndex)
        {
            caretIndex ??= CaretIndex;

            Rect hitTestTextPosition = TextLayout.HitTestTextPosition(caretIndex.Value);
            CaretPosition = new Point(hitTestTextPosition.X, hitTestTextPosition.Y);
            return hitTestTextPosition;
        }
    }
}