using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Utilities;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    // This is workaround for Avalonia issue https://github.com/AvaloniaUI/AvaloniaEdit/issues/540
    // Wheel mouse values are hard coded.
    public class ConsoleScrollContentPresenter : ScrollContentPresenter
    {
        // Cache MethodInfo for private base method to avoid repeated reflection lookup.
        private static readonly MethodInfo SnapOffsetMethod =
            typeof(ScrollContentPresenter).GetMethod("SnapOffset",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            // base.OnPointerWheelChanged(e);
            if (Extent.Height > Viewport.Height || Extent.Width > Viewport.Width)
            {
                // var scrollable = Child as ILogicalScrollable;
                // var isLogical = scrollable?.IsLogicalScrollEnabled == true;

                double x = Offset.X;
                double y = Offset.Y;
                Vector delta = e.Delta;

                // KeyModifiers.Shift should scroll in horizontal direction. This does not work on every platform. 
                // If Shift-Key is pressed and X is close to 0 we swap the Vector.
                // NOTE: Changed to also include CTRL
                if ((e.KeyModifiers == KeyModifiers.Control || e.KeyModifiers == KeyModifiers.Shift) &&
                    MathUtilities.IsZero(delta.X))
                    delta = new Vector(delta.Y, delta.X);
                else
                    delta = AdjustDeltaForFlowDirection(delta, FlowDirection);

                if (Extent.Height > Viewport.Height)
                {
                    // double height = isLogical ? scrollable!.ScrollSize.Height : 50;
                    double height = 3;
                    y += -delta.Y * height;
                    y = Math.Max(y, 0);
                    y = Math.Min(y, Extent.Height - Viewport.Height);
                }

                if (Extent.Width > Viewport.Width)
                {
                    //double width = isLogical ? scrollable!.ScrollSize.Width : 50;
                    double width = 3;
                    x += -delta.X * width;
                    x = Math.Max(x, 0);
                    x = Math.Min(x, Extent.Width - Viewport.Width);
                }

                var newOffset = (Vector)SnapOffsetMethod.Invoke(this, new object[] { new Vector(x, y), delta, true })!;

                bool offsetChanged = newOffset != Offset;
                SetCurrentValue(OffsetProperty, newOffset);

                e.Handled = !IsScrollChainingEnabled || offsetChanged;
            }
        }

        private static Vector AdjustDeltaForFlowDirection(Vector delta, FlowDirection flowDirection)
        {
            if (flowDirection == FlowDirection.RightToLeft) return delta.WithX(-delta.X);
            return delta;
        }
    }
}