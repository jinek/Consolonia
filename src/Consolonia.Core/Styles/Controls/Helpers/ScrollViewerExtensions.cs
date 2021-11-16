using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    internal class ScrollViewerExtensions : AvaloniaObject
    {
        public static readonly AttachedProperty<bool> ScrollOnArrowsProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, bool>("ScrollOnArrows", typeof(ScrollViewerExtensions));

        public static readonly AttachedProperty<GridLength> ScrollBarsWidthProperty =
            AvaloniaProperty.RegisterAttached<ScrollViewer, GridLength>("ScrollBarsWidth",
                typeof(ScrollViewerExtensions));

        static ScrollViewerExtensions()
        {
            ScrollBarsWidthProperty.Changed.Subscribe(args =>
            {
                var scrollViewer = (ScrollViewer)args.Sender;
                var grid = (Grid)scrollViewer.GetTemplateChildren().SingleOrDefault(control => control.Name == @"PART_Root");
                if (grid != null) Apply();
                else
                {
                    void OnScrollViewerOnTemplateApplied(object sender, TemplateAppliedEventArgs eventArgs)
                    {
                        scrollViewer.TemplateApplied -= OnScrollViewerOnTemplateApplied;
                        grid = (Grid)scrollViewer.GetTemplateChildren().SingleOrDefault(control => control.Name == @"PART_Root");
                        Apply();
                    }

                    scrollViewer.TemplateApplied += OnScrollViewerOnTemplateApplied;
                }
                void Apply()
                {
                    grid.RowDefinitions[1].Height = args.NewValue.Value;
                    grid.ColumnDefinitions[1].Width = args.NewValue.Value;
                }
            });
            
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

        public GridLength ScrollBarsWidth
        {
            get => GetValue(ScrollBarsWidthProperty);
            set => SetValue(ScrollOnArrowsProperty, value);
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