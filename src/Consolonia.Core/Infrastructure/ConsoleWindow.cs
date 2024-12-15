using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Text;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoleWindow : IWindowImpl
    {
        private readonly IKeyboardDevice _myKeyboardDevice;
        private readonly TimeSpan _resizeDelay = TimeSpan.FromMilliseconds(100);
        [NotNull] internal readonly IConsole Console;
        private bool _disposedValue;
        private IInputRoot _inputRoot;
        private CancellationTokenSource _resizeCancellationTokenSource;

        public ConsoleWindow()
        {
            _myKeyboardDevice = AvaloniaLocator.Current.GetService<IKeyboardDevice>();
            MouseDevice = AvaloniaLocator.Current.GetService<IMouseDevice>();
            Console = AvaloniaLocator.Current.GetService<IConsole>() ?? throw new NotImplementedException();
            Console.Resized += OnConsoleOnResized;
            Console.KeyEvent += ConsoleOnKeyEvent;
            Console.MouseEvent += ConsoleOnMouseEvent;
            Console.FocusEvent += ConsoleOnFocusEvent;
            Handle = null!;
            PixelBuffer = new PixelBuffer(Console.Size);
        }

        public PixelBuffer PixelBuffer { get; private set; }

        private IMouseDevice MouseDevice { get; }


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

        public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels)
        {
            Debug.WriteLine($"ConsoleWindow.SetTransparencyLevelHint({transparencyLevels}) called, not implemented");
        }

        public void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
        {
            //todo: Light or dark
            switch (themeVariant)
            {
                case PlatformThemeVariant.Dark:
                    Debug.WriteLine($"ConsoleWindow.SetFrameThemeVariant({themeVariant}) called, not implemented");
                    break;
                case PlatformThemeVariant.Light:
                    Debug.WriteLine($"ConsoleWindow.SetFrameThemeVariant({themeVariant}) called, not implemented");
                    break;
            }
        }

        public Size ClientSize
        {
            get
            {
                PixelBufferSize pixelBufferSize = Console.Size;
                return new Size(pixelBufferSize.Width, pixelBufferSize.Height);
            }
        }

        public Size? FrameSize => ClientSize;

        public double RenderScaling => 1;
        public IEnumerable<object> Surfaces => [this];

        public Action<RawInputEventArgs> Input { get; set; }

        public Action<Rect> Paint { get; set; }
        public Action<Size, WindowResizeReason> Resized { get; set; }


        public Action<double> ScalingChanged { get; set; }

        public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }

        public Compositor Compositor { get; } = new(null);
        public Action Closed { get; set; }
        public Action LostFocus { get; set; }

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.None;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new(1, 1, 1);

        public void Show(bool activate, bool isDialog)
        {
            if (activate)
                Activated!();
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
            // todo
            Debug.WriteLine($"ConsoleWindow.SetTopmost({value}) called, not implemented");
        }

        public double DesktopScaling => 1d;

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

        public void SetTitle(string title)
        {
            Console.SetTitle(title);
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
            // todo, enable/disable resizing of window
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Resize(Size clientSize, WindowResizeReason reason = WindowResizeReason.Application)
        {
            //todo: can we deny resizing?
        }


        public void Move(PixelPoint point)
        {
            throw new NotImplementedException();
        }

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            //throw new NotImplementedException();
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {
            // we don't support this, we can ignore
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {
            // we don't support this, we can ignore
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {
            // we don't support this, we can ignore
        }

        public WindowState WindowState { get; set; }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Action GotInputWhenDisabled { get; set; }
        public Func<WindowCloseReason, bool> Closing { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public bool IsClientAreaExtendedToDecorations { get; }
        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public bool NeedsManagedDecorations { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Thickness ExtendedMargins { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Thickness OffScreenMargin { get; }

        public object TryGetFeature(Type featureType)
        {
            if (featureType == typeof(IStorageProvider))
                return AvaloniaLocator.Current.GetService<IStorageProvider>();

            if (featureType == typeof(IInsetsManager))
                // IInsetsManager doesn't apply to console applications.
                return null;

            if (featureType == typeof(IClipboard))
            {
                var clipboard = AvaloniaLocator.CurrentMutable.GetService<IClipboard>();
                if (clipboard != null)
                    return clipboard;
            }

            // TODO ISystemNavigationManagerImpl should be implemented to handle BACK navigation between pages of controls like mobile apps do.
            // TODO ITextInputMethodImpl should be implemented to handle text IME input
            Debug.WriteLine($"Missing Feature: {featureType.Name} is not implemented but someone is asking for it!");
            return null;
        }

        public void GetWindowsZOrder(Span<Window> windows, Span<long> zOrder)
        {
            // In console mode, all windows are considered to be at the same z-order level
            for (int i = 0; i < zOrder.Length; i++)
                zOrder[i] = 0;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ConsoleWindow()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ConsoleOnMouseEvent(RawPointerEventType type, Point point, Vector? wheelDelta,
            RawInputModifiers modifiers)
        {
            ulong timestamp = (ulong)Stopwatch.GetTimestamp();
            Dispatcher.UIThread.Post(() =>
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
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
                        Input!(new RawPointerEventArgs(MouseDevice, timestamp, _inputRoot,
                            type, point,
                            modifiers));
                        break;
                    case RawPointerEventType.Wheel:
                        Input!(new RawMouseWheelEventArgs(MouseDevice, timestamp, _inputRoot, point,
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
            // clear screen so we don't see crazy while resizing.
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            System.Console.Write(Esc.ClearScreen);
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            // Cancel previous task if there is one and start a new one
            CancellationTokenSource oldCts = _resizeCancellationTokenSource;
            _resizeCancellationTokenSource = new CancellationTokenSource();
            oldCts?.Cancel();
            oldCts?.Dispose();

            // ReSharper disable once GrammarMistakeInComment
            // start a task which if no resize event comes for _resizeDelay will post the resize to the window
            // ReSharper disable once MethodSupportsCancellation
            Task.Run(async () =>
            {
                try
                {
                    // Wait for the delay period, this task will be canceled if another refresh comes in.
                    await Task.Delay(_resizeDelay, _resizeCancellationTokenSource.Token);

                    // dispatch to the UI thread 
                    Dispatcher.UIThread.Post(() =>
                    {
                        PixelBuffer = new PixelBuffer(Console.Size);
                        var size = new Size(Console.Size.Width, Console.Size.Height);
                        Resized!(size, WindowResizeReason.Unspecified);
                    });
                }
                catch (TaskCanceledException)
                {
                    // Ignore cancellation exception, we want this to happen while resizes are happening quickly
                }
            });
        }

        private async void ConsoleOnKeyEvent(Key key, char keyChar, RawInputModifiers rawInputModifiers, bool down,
            ulong timeStamp)
        {
            if (!down)
            {
                Dispatcher.UIThread.Post(() =>
                {
#pragma warning disable CS0618 // Type or member is obsolete // todo: change to correct constructor, CFA20A9A-3A24-4187-9CA3-9DF0081124EE 
                    var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp, _inputRoot,
                        RawKeyEventType.KeyUp, key,
                        rawInputModifiers);
#pragma warning restore CS0618 // Type or member is obsolete
                    Input!(rawInputEventArgs);
                }, DispatcherPriority.Input);
            }
            else
            {
                bool handled = false;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
#pragma warning disable CS0618 // Type or member is obsolete //todo: CFA20A9A-3A24-4187-9CA3-9DF0081124EE
                    var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp,
                        _inputRoot,
                        RawKeyEventType.KeyDown, key,
                        rawInputModifiers);
#pragma warning restore CS0618 // Type or member is obsolete
                    Input!(rawInputEventArgs);
                    handled = rawInputEventArgs.Handled;
                }, DispatcherPriority.Input);

                if (!handled
                    && !char.IsControl(keyChar)
                    && !rawInputModifiers.HasFlag(RawInputModifiers.Alt)
                    && !rawInputModifiers.HasFlag(RawInputModifiers.Control))
                    Dispatcher.UIThread.Post(() =>
                    {
                        Input!(new RawTextInputEventArgs(_myKeyboardDevice,
                            timeStamp,
                            _inputRoot,
                            keyChar.ToString()));
                    }, DispatcherPriority.Input);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Closed?.Invoke();
                    Console.Resized -= OnConsoleOnResized;
                    Console.KeyEvent -= ConsoleOnKeyEvent;
                    Console.MouseEvent -= ConsoleOnMouseEvent;
                    Console.FocusEvent -= ConsoleOnFocusEvent;
                    Console.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}