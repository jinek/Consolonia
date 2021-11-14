using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    internal static class ScrollViewerExtensions
    {
        public static readonly AttachedProperty<bool> ScrollOnArrowsProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, bool>("ScrollOnArrows", typeof(ButtonExtensions));

        static ScrollViewerExtensions()
        {
            ScrollOnArrowsProperty.Changed.Subscribe(args =>
            {
                var scrollViewer = (ScrollViewer)args.Sender;
                if (args.NewValue.Value)
                {
                    scrollViewer.KeyDown += ScrollViewerOnKeyDown;
                }
                else
                {
                    scrollViewer.KeyDown -= ScrollViewerOnKeyDown;
                }
            });
        }

        private static void ScrollViewerOnKeyDown(object? sender, KeyEventArgs e)
        {
            var scrollViewer = (ScrollViewer)sender;
            if (!scrollViewer.Focusable)
                return;

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (e.Key)
            {
                case Key.Up:
                    scrollViewer.Offset = scrollViewer.Offset.WithY(scrollViewer.Offset.Y - 1);
                    e.Handled = true;
                    break;
                case Key.Down:
                    scrollViewer.Offset = scrollViewer.Offset.WithY(scrollViewer.Offset.Y + 1);
                    e.Handled = true;
                    break;
                case Key.Left:
                    scrollViewer.Offset = scrollViewer.Offset.WithX(scrollViewer.Offset.X - 1);
                    e.Handled = true;
                    break;
                case Key.Right:
                    scrollViewer.Offset = scrollViewer.Offset.WithX(scrollViewer.Offset.X + 1);
                    e.Handled = true;
                    break;
            }
        }
    }
}