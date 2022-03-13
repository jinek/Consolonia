using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.VisualTree;

namespace Consolonia.Core.Dummy
{
    internal class DummyMouse : IMouseDevice
    {
        public void ProcessRawEvent(RawInputEventArgs ev)
        {
        }

        public void Capture(IInputElement control)
        {
        }

        public Point GetPosition(IVisual relativeTo)
        {
            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident todo: why resharper suggests this only on github action?
            return new();
        }

        public IInputElement Captured => null;

        public void TopLevelClosed(IInputRoot root)
        {
        }

        public void SceneInvalidated(IInputRoot root, Rect rect)
        {
            // now not drawing the mouse
        }

        public PixelPoint Position => PixelPoint.Origin;
    }
}