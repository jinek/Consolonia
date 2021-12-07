using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBuffer;
using Consolonia.Core.Dummy;

namespace Consolonia.Core.Infrastructure
{
    internal class ConsoleWindow : IWindowImpl
    {
        private readonly IConsole _console;
        private readonly IKeyboardDevice _myKeyboardDevice;
        internal readonly List<Rect> InvalidatedRects = new(50);
        private IInputRoot _inputRoot;

        public ConsoleWindow()
        {
            _myKeyboardDevice = AvaloniaLocator.Current.GetService<IKeyboardDevice>();
            _console = AvaloniaLocator.Current.GetService<IConsole>();

            _console = AvaloniaLocator.Current.GetService<IConsole>();
            _console.Resized += () =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    PixelBufferSize pixelBufferSize = _console.Size;
                    Resized(new Size(pixelBufferSize.Width, pixelBufferSize.Height));
                });
            };

            _console.KeyPress += ConsoleOnKeyPress;
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IRenderer CreateRenderer(IRenderRoot root)
        {
            /*return new X11ImmediateRendererProxy(root, AvaloniaLocator.Current.GetService<IRenderLoop>())
                { DrawDirtyRects = false, DrawFps = false };*/
            return new AdvancedDeferredRenderer(root, AvaloniaLocator.Current.GetService<IRenderLoop>())
            {
                RenderRoot = this
//                RenderOnlyOnRenderThread = true
            };
        }

        public void Invalidate(Rect rect)
        {
            if (rect.IsEmpty) return;
            InvalidatedRects.Add(rect);


            /*
             This is the code for drawing invalid rectangles
             var _console = AvaloniaLocator.Current.GetService<IConsole>();
            using (_console.StoreCaret())
            {
                for (int y = (int)rect.Y; y < rect.Bottom; y++)
                {
                    if (y < Console.WindowHeight - 2)
                    {
                        Console.SetCursorPosition((int)rect.X, y);
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.Magenta;

                        Console.WriteLine(string.Concat(Enumerable.Range(0, (int)rect.Width).Select(i => ' ')));
                    }
                }
            }*/
            //Paint(new Rect(0, 0, ClientSize.Width, ClientSize.Height));

            //Paint(new Rect(rect.Left, rect.Top, rect.Width, rect.Height));
        }

        public void SetInputRoot(IInputRoot inputRoot)
        {
            _inputRoot = inputRoot;
        }

        public Point PointToClient(PixelPoint point)
        {
            return point.ToPoint(1);
        }

        public PixelPoint PointToScreen(Point point)
        {
            throw new NotImplementedException();
        }

        public void SetCursor(ICursorImpl cursor)
        {
            throw new NotImplementedException();
        }

        public IPopupImpl CreatePopup()
        {
            return null; // when returning null top window overlay layer will be used
        }

        public void SetTransparencyLevelHint(WindowTransparencyLevel transparencyLevel)
        {
        }

        public Size ClientSize
        {
            get
            {
                PixelBufferSize pixelBufferSize = _console.Size;
                return new Size(pixelBufferSize.Width, pixelBufferSize.Height);
            }
        }

        public double RenderScaling => 1;
        public IEnumerable<object> Surfaces => new[] { this };

        public Action<RawInputEventArgs> Input { get; set; }

        public Action<Rect> Paint { get; set; }

        public Action<Size> Resized { get; set; }

        public Action<double> ScalingChanged { get; set; }

        public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }

        public Action Closed { get; set; }
        public Action LostFocus { get; set; }
        public IMouseDevice MouseDevice => new DummyMouse();

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.None;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new(1, 1, 1);

        public void Show(bool activate)
        {
            if (activate)
                Activated();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void SetTopmost(bool value)
        {
            throw new NotImplementedException();
        }

        public double DesktopScaling { get; } = 1d;
        public PixelPoint Position { get; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }
        public IPlatformHandle Handle { get; }
        public Size MaxAutoSizeHint { get; }
        public IScreenImpl Screen { get; }

        public void SetTitle(string title)
        {
            Console.Title = title;
        }

        public void SetParent(IWindowImpl parent)
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool enable)
        {
            throw new NotImplementedException();
        }

        public void SetSystemDecorations(SystemDecorations enabled)
        {
            throw new NotImplementedException();
        }

        public void SetIcon(IWindowIconImpl icon)
        {
        }

        public void ShowTaskbarIcon(bool value)
        {
        }

        public void CanResize(bool value)
        {
            throw new NotImplementedException();
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Resize(Size clientSize)
        {
            //support only autosizing for now
        }

        public void Move(PixelPoint point)
        {
            throw new NotImplementedException();
        }

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {
            throw new NotImplementedException();
        }

        public WindowState WindowState { get; set; }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Action GotInputWhenDisabled { get; set; }
        public Func<bool> Closing { get; set; }
        public bool IsClientAreaExtendedToDecorations { get; }
        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }
        public bool NeedsManagedDecorations { get; }
        public Thickness ExtendedMargins { get; }
        public Thickness OffScreenMargin { get; }

        private async void ConsoleOnKeyPress(Key key, char keyChar, RawInputModifiers rawInputModifiers)
        {
            bool handled = false;
            if (!char.IsControl(keyChar))
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var rawTextInputEventArgs = new RawTextInputEventArgs(_myKeyboardDevice, (ulong)DateTime.Now.Ticks,
                        _inputRoot,
                        keyChar.ToString());
                    Input(rawTextInputEventArgs);
                    if (rawTextInputEventArgs.Handled)
                        handled = true;
                });

            if (handled) return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Input(new RawKeyEventArgs(_myKeyboardDevice, (ulong)DateTime.Now.Ticks, _inputRoot,
                    RawKeyEventType.KeyDown, key,
                    rawInputModifiers));
            });
            
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Input(new RawKeyEventArgs(_myKeyboardDevice, (ulong)DateTime.Now.Ticks, _inputRoot,
                    RawKeyEventType.KeyUp, key,
                    rawInputModifiers));
            });
        }
    }
}