using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using Consolonia.Core.InternalHelpers;

namespace Consolonia.Core.Infrastructure
{
    public class ArrowsAndKeyboardNavigationHandler : IKeyboardNavigationHandler
    {
        private readonly IKeyboardNavigationHandler _keyboardNavigationHandler;

        //todo: check XTFocus https://github.com/jinek/Consolonia/issues/105#issuecomment-2089015880
        private IInputRoot _owner;

        public ArrowsAndKeyboardNavigationHandler()
        {
            _keyboardNavigationHandler = new KeyboardNavigationHandler();
        }

        public void SetOwner(IInputRoot owner)
        {
            _keyboardNavigationHandler.SetOwner(owner);
            //todo: should we RemoveHandler here?
            _owner = owner;
            _owner.AddHandler(InputElement.KeyDownEvent, new EventHandler<KeyEventArgs>(OnKeyDown));
        }

        public void Move(IInputElement element, NavigationDirection direction,
            KeyModifiers keyModifiers = KeyModifiers.None)
        {
            if (direction is NavigationDirection.Right or
                NavigationDirection.Left or
                NavigationDirection.Down or
                NavigationDirection.Up)
            {
                var elementCast = (InputElement)element;
                var visualRoot = (Visual)elementCast.GetVisualRoot();
                (Point p1, Point p2) = GetOriginalPoint(elementCast.GetTransformedBounds().NotNull().Clip);
                Point originalPoint = p1 / 2 + p2 / 2;

                var focusableElements = visualRoot!.GetVisualDescendants()
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
                            GetTargetPoint(inputElement.GetTransformedBounds().NotNull().Clip);
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
                    // selecting the closest one
                    .MinBy(arg =>
                    {
                        (double x, double y) = arg.vector;
                        return x * x + y * y;
                    });

                focusableElements?.inputElement?.Focus();
            }
            else
            {
                _keyboardNavigationHandler.Move(element, direction, keyModifiers);
            }

            return;

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

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled) return;

            if (e.Key == Key.Escape)
            {
                // if there is a overlay popup, close it
                var overlay = ((Visual)sender).FindDescendantOfType<OverlayPopupHost>();
                if (overlay != null)
                {
                    // it will have a popup as the parent.
                    var popup = overlay.Parent as Popup;
                    if (popup != null)
                        popup.Close();
                    e.Handled = true;
                    return;
                }
            }

            //see FocusManager.GetFocusManager
            IInputElement current = TopLevel.GetTopLevel((Visual)sender)!.FocusManager!.GetFocusedElement();

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