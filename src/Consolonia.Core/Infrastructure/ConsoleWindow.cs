using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using JetBrains.Annotations;

namespace Consolonia.Core.Infrastructure
{
    internal class ConsoleWindow : IWindowImpl
    {
        [NotNull] private readonly IConsole _console;
        private readonly IKeyboardDevice _myKeyboardDevice;
        internal readonly List<Rect> InvalidatedRects = new(50);
        private IInputRoot _inputRoot;

        public ConsoleWindow()
        {
            _myKeyboardDevice = AvaloniaLocator.Current.GetService<IKeyboardDevice>();
            MouseDevice = AvaloniaLocator.Current.GetService<IMouseDevice>();
            _console = AvaloniaLocator.Current.GetService<IConsole>() ?? throw new NotImplementedException();
            _console.Resized += OnConsoleOnResized;
            _console.KeyEvent += ConsoleOnKeyEvent;
            _console.MouseEvent += ConsoleOnMouseEvent;
            _console.FocusEvent += ConsoleOnFocusEvent;
        }

        public void Dispose()
        {
            Closed?.Invoke();
            _console.Resized -= OnConsoleOnResized;
            _console.KeyEvent -= ConsoleOnKeyEvent;
            _console.MouseEvent -= ConsoleOnMouseEvent;
            _console.FocusEvent -= ConsoleOnFocusEvent;
            _console.Dispose();
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
            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident //todo: should we avoid suggesting type specification
            return new((int)point.X, (int)point.Y);
        }

        public void SetCursor(ICursorImpl cursor)
        {
            //todo: check whether we can work with cursors
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

        public Size? FrameSize => ClientSize;

        public double RenderScaling => 1;
        public IEnumerable<object> Surfaces => new[] { this };

        public Action<RawInputEventArgs> Input { get; set; }

        public Action<Rect> Paint { get; set; }
        public Action<Size, PlatformResizeReason> Resized { get; set; }

        public Action<double> ScalingChanged { get; set; }

        public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }

        public Action Closed { get; set; }
        public Action LostFocus { get; set; }
        public IMouseDevice MouseDevice { get; }

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.None;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new(1, 1, 1);

        public void Show(bool activate, bool isDialog)
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

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: get from _console if supported
        public PixelPoint Position { get; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: get from _console if supported
        public IPlatformHandle Handle { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Size MaxAutoSizeHint { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public IScreenImpl Screen { get; }

        public void SetTitle(string title)
        {
            _console.SetTitle(title);
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

        public void Resize(Size clientSize, PlatformResizeReason reason = PlatformResizeReason.Application)
        {
            Resized(clientSize, reason);
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

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public bool IsClientAreaExtendedToDecorations { get; }
        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public bool NeedsManagedDecorations { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Thickness ExtendedMargins { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Thickness OffScreenMargin { get; }

        private void ConsoleOnMouseEvent(RawPointerEventType type, Point point, Vector? wheelDelta,
            RawInputModifiers modifiers)
        {
            ulong timestamp = (ulong)Stopwatch.GetTimestamp();
            Dispatcher.UIThread.Post(() =>
            {
                switch (type)
                {
                    case RawPointerEventType.Move:
                    case RawPointerEventType.LeftButtonDown:
                    case RawPointerEventType.LeftButtonUp:
                    case RawPointerEventType.RightButtonUp:
                    case RawPointerEventType.RightButtonDown:
                    case RawPointerEventType.MiddleButtonDown:
                    case RawPointerEventType.XButton1Down:
                    case RawPointerEventType.XButton2Down:
                    case RawPointerEventType.NonClientLeftButtonDown:
                    case RawPointerEventType.MiddleButtonUp:
                    case RawPointerEventType.XButton1Up:
                    case RawPointerEventType.XButton2Up:
                        Input(new RawPointerEventArgs(MouseDevice, timestamp, _inputRoot,
                            type, point,
                            modifiers));
                        break;
                    case RawPointerEventType.Wheel:
                        Input(new RawMouseWheelEventArgs(MouseDevice, timestamp, _inputRoot, point,
                            (Vector)wheelDelta!, modifiers));
                        break;
                }
            });
        }

        private void ConsoleOnFocusEvent(bool focused)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (focused)
                    Activated?.Invoke();
                else Deactivated?.Invoke();
            });
        }

        private void OnConsoleOnResized()
        {
            Dispatcher.UIThread.Post(() =>
            {
                PixelBufferSize pixelBufferSize = _console.Size;
                var size = new Size(pixelBufferSize.Width, pixelBufferSize.Height);
                Resized(size, PlatformResizeReason.Unspecified);
                //todo; Invalidate(new Rect(size));
            });
        }

        private async void ConsoleOnKeyEvent(Key key, char keyChar, RawInputModifiers rawInputModifiers, bool down,
            ulong timeStamp)
        {
            if (!down)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    Input(new RawKeyEventArgs(_myKeyboardDevice, timeStamp, _inputRoot,
                        RawKeyEventType.KeyUp, key,
                        rawInputModifiers));
                }, DispatcherPriority.Input);
            }
            else
            {
                bool handled = false;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp,
                        _inputRoot,
                        RawKeyEventType.KeyDown, key,
                        rawInputModifiers);
                    Input(rawInputEventArgs);
                    handled = rawInputEventArgs.Handled;
                }, DispatcherPriority.Input).ConfigureAwait(true);

                if (!handled && !char.IsControl(keyChar))
                    Dispatcher.UIThread.Post(() =>
                    {
                        Input(new RawTextInputEventArgs(_myKeyboardDevice,
                            timeStamp,
                            _inputRoot,
                            keyChar.ToString()));
                    }, DispatcherPriority.Input);
            }
        }
    }
}