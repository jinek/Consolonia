using System;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Rendering;
using Avalonia.VisualTree;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Infrastructure
{
    public class ArrowsAndKeyboardNavigationHandler : KeyboardNavigationHandler, IKeyboardNavigationHandler
    {
        public new void Move(IInputElement element, NavigationDirection direction,
            KeyModifiers keyModifiers = KeyModifiers.None)
        {
            if (direction is NavigationDirection.Right or
                NavigationDirection.Left or
                NavigationDirection.Down or
                NavigationDirection.Up)
            {
                IRenderRoot visualRoot = element.GetVisualRoot();
                (Point p1, Point p2) = GetOriginalPoint(element.TransformedBounds.NotNull().Clip);
                Point originalPoint = p1 / 2 + p2 / 2;

                var focusableElements = visualRoot.GetVisualDescendants()
                    .OfType<InputElement>()
                    // only focusable
                    .Where(inputElement => inputElement.Focusable && inputElement.IsEffectivelyEnabled &&
                                           inputElement.IsEffectivelyVisible &&
                                           inputElement.IsTabStop
                    )
                    // calculating vector between old focus point and candidate point
                    .Select(inputElement =>
                    {
                        (Point firstTargetPoint, Point secondTargetPoint) =
                            GetTargetPoint(inputElement.TransformedBounds.NotNull().Clip);
                        return new
                        {
                            vector =
                                firstTargetPoint / 2 +
                                secondTargetPoint / 2
                                - originalPoint,
                            inputElement
                        };
                    })
                    // only with in a cone
                    .Where(arg =>
                    {
                        Point vector = arg.vector;
                        return IsInCone(vector);
                    })
                    // selecting closest one
                    .MinBy(arg =>
                    {
                        Point coordinates = arg.vector;
                        return coordinates.X * coordinates.X + coordinates.Y * coordinates.Y;
                    });

                focusableElements?.inputElement.Focus();
            }
            else
            {
                base.Move(element, direction, keyModifiers);
            }

            (Point, Point) GetOriginalPoint(Rect valueClip)
            {
#pragma warning disable CS8509
                return direction switch
                {
                    NavigationDirection.Right => (valueClip.BottomRight, valueClip.TopRight),
                    NavigationDirection.Left => (valueClip.BottomLeft, valueClip.TopLeft),
                    NavigationDirection.Up => (valueClip.TopRight, valueClip.TopLeft),
                    NavigationDirection.Down => (valueClip.BottomRight, valueClip.BottomLeft)
                };
            }

            (Point, Point) GetTargetPoint(Rect clip)
            {
                return direction switch
                {
                    NavigationDirection.Right => (clip.TopLeft, clip.BottomLeft),
                    NavigationDirection.Left => (clip.TopRight, clip.BottomRight),
                    NavigationDirection.Up => (clip.BottomLeft, clip.BottomRight),
                    NavigationDirection.Down => (clip.TopLeft, clip.TopRight)
                };
            }

            bool IsInCone(Point vector)
            {
                return direction switch
#pragma warning restore CS8509
                {
                    NavigationDirection.Right => vector.X >= Math.Abs(vector.Y) / 2,
                    NavigationDirection.Left => -vector.X >= Math.Abs(vector.Y) / 2,
                    NavigationDirection.Down => vector.Y >= Math.Abs(vector.X) / 8,
                    NavigationDirection.Up => -vector.Y >= Math.Abs(vector.X) / 8
                };
            }
        }

        protected override void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(sender, e);

            if (e.Handled) return;
            IInputElement current = FocusManager.Instance?.Current;

            if (e.KeyModifiers != KeyModifiers.None)
                return;

            if (current == null) return;

            NavigationDirection direction;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (e.Key)
            {
                case Key.Right:
                    direction = NavigationDirection.Right;
                    break;
                case Key.Left:
                    direction = NavigationDirection.Left;
                    break;
                case Key.Up:
                    direction = NavigationDirection.Up;
                    break;
                case Key.Down:
                    direction = NavigationDirection.Down;
                    break;
                default: return;
            }

            Move(current, direction, e.KeyModifiers);
            e.Handled = true;
        }
    }
}