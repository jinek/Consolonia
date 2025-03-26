using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.Raw;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Rendering.Composition;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Helpers;
using Window = Avalonia.Controls.Window;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     ConsoleWindow - a TopLevel which uses the ConsoleWindowImpl to interact with the console.
    /// </summary>
    /// <remarks>
    ///     This window content is the MainView
    /// </remarks>
    public class ConsoleWindow : Window
    {
        public ConsoleWindow() :
            this(new ConsoleWindowImpl())

        {
        }

        public ConsoleWindow(IWindowImpl impl)
            : base(impl)
        {
        }

        public Control MainView
        {
            get => Content as Control;
            set => Content = value;
        }
    }

    /// <summary>
    ///     ConsoleWindowImpl - An IWindowImpl which uses a PixelBuffer to render.
    /// </summary>
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class ConsoleWindowImpl : IWindowImpl
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly bool _accessKeysAlwaysOn;
        private readonly IDisposable _accessKeysAlwaysOnDisposable;
        private readonly IKeyboardDevice _myKeyboardDevice;
        private readonly List<Rect> _refreshRects = new();

        [NotNull] internal readonly IConsole Console;

        private StandardCursorType _cursorType = StandardCursorType.Arrow;
        private bool _disposedValue;
        private IInputRoot _inputRoot;

        public ConsoleWindowImpl()
        {
            _myKeyboardDevice = AvaloniaLocator.Current.GetService<IKeyboardDevice>();
            MouseDevice = AvaloniaLocator.Current.GetService<IMouseDevice>();
            Console = AvaloniaLocator.Current.GetService<IConsole>() ?? throw new NotImplementedException();
            PixelBuffer = new PixelBuffer(Console.Size);
            Console.Resized += OnConsoleOnResized;
            Console.KeyEvent += ConsoleOnKeyEvent;
            Console.TextInputEvent += ConsoleOnTextInputEvent;
            Console.MouseEvent += ConsoleOnMouseEvent;
            Console.FocusEvent += ConsoleOnFocusEvent;
            Handle = null!;
            _accessKeysAlwaysOn = !Console.SupportsAltSolo;
            if (_accessKeysAlwaysOn)
                _accessKeysAlwaysOnDisposable =
                    UtilityExtensions.SubscribeAction(AccessText.ShowAccessKeyProperty.Changed,
                        OnShowAccessKeyPropertyChanged);
        }

        public PixelBuffer PixelBuffer { get; private set; }

        private IMouseDevice MouseDevice { get; }


        public void SetInputRoot(IInputRoot inputRoot)
        {
            _inputRoot = inputRoot;
            if (_accessKeysAlwaysOn)
                _inputRoot.ShowAccessKeys = true;
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
            if (cursor is null)
                // default to arrow
                _cursorType = StandardCursorType.Arrow;
            else
                _cursorType = ((CursorImpl)cursor).CursorType;
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
            // toplevel never hides
            throw new NotImplementedException();
        }

        public void Activate()
        {
            // toplevel is always visible
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
            throw new NotSupportedException("You can't set a toplevel parent");
        }

        public void SetEnabled(bool enable)
        {
            throw new NotImplementedException();
        }

        public void SetSystemDecorations(SystemDecorations enabled)
        {
            // console window has no system decorations
        }

        public void SetIcon(IWindowIconImpl icon)
        {
            // console window has no icon
        }

        public void ShowTaskbarIcon(bool value)
        {
            // console window has no icon
        }

        public void CanResize(bool value)
        {
            // console app can't do this.
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            // console app can't do this.
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            // console app can't do this.
        }

        public void Resize(Size clientSize, WindowResizeReason reason = WindowResizeReason.Application)
        {
            // console app can't do this.
        }

        public void Move(PixelPoint point)
        {
            // console app can't do this.
        }

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            // console app can't do this.
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {
            // console app can't do this.
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {
            // console app can't do this.
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {
            // console app can't do this.
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
                return new ConsoloniaStorageProvider();

            if (featureType == typeof(IInsetsManager))
                // IInsetsManager doesn't apply to console applications.
                return null;

            if (featureType == typeof(IClipboard))
            {
                var clipboard = AvaloniaLocator.CurrentMutable.GetService<IClipboard>();
                if (clipboard != null)
                    return clipboard;
            }

            if (featureType == typeof(IScreenImpl))
                return new ConsoloniaScreen(new PixelRect(0, 0, Console.Size.Width, Console.Size.Height));

            if (featureType == typeof(ILauncher))
            {
                ObjectHandle objHandle =
                    Activator.CreateInstance("Avalonia.Base", "Avalonia.Platform.Storage.FileIO.BclLauncher");
                return (ILauncher)objHandle.Unwrap();
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

        private void OnShowAccessKeyPropertyChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            if (args.Sender != _inputRoot) return;
            if (args.GetNewValue<bool>()) return;

            _inputRoot.ShowAccessKeys = true;
        }

        private void ConsoleOnMouseEvent(RawPointerEventType type, Point point, Vector? wheelDelta,
            RawInputModifiers modifiers)
        {
            ulong timestamp = (ulong)Environment.TickCount64;
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

            // draw the software cursor at this mouse position
            RenderSoftwareCursor(point);
        }

        private void ConsoleOnFocusEvent(bool focused)
        {
            if (focused)
                Activated?.Invoke();
            else Deactivated?.Invoke();
        }

        private void OnConsoleOnResized()
        {
            var size = new Size(Console.Size.Width, Console.Size.Height);
            PixelBuffer = new PixelBuffer((ushort)size.Width, (ushort)size.Height);
            Resized!(size, WindowResizeReason.Unspecified);
        }

        private void ConsoleOnTextInputEvent(string text, ulong timeStamp, CanBeHandledEventArgs canBeHandledEventArgs)
        {
#pragma warning disable CS0618 // Type or member is obsolete // todo: change to correct constructor, CFA20A9A-3A24-4187-9CA3-9DF0081124EE 
            RawTextInputEventArgs rawInputEventArgs = new(_myKeyboardDevice, timeStamp, _inputRoot, text);
#pragma warning restore CS0618 // Type or member is obsolete
            Input!(rawInputEventArgs);

            if (rawInputEventArgs.Handled)
                canBeHandledEventArgs.Handled = true;
        }


        private void ConsoleOnKeyEvent(Key key, char keyChar, RawInputModifiers rawInputModifiers, bool down,
            ulong timeStamp, bool tryAsTextInput)
        {
            if (!down)
            {
#pragma warning disable CS0618 // Type or member is obsolete // todo: change to correct constructor, CFA20A9A-3A24-4187-9CA3-9DF0081124EE 
                var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp, _inputRoot,
                    RawKeyEventType.KeyUp, key,
                    rawInputModifiers);
#pragma warning restore CS0618 // Type or member is obsolete
                Input!(rawInputEventArgs);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete //todo: CFA20A9A-3A24-4187-9CA3-9DF0081124EE
                var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp,
                    _inputRoot,
                    RawKeyEventType.KeyDown, key,
                    rawInputModifiers);
#pragma warning restore CS0618 // Type or member is obsolete
                Input!(rawInputEventArgs);

                if (tryAsTextInput &&
                    !rawInputEventArgs.Handled
                    && !char.IsControl(keyChar)
                    && !rawInputModifiers.HasFlag(RawInputModifiers.Alt)
                    && !rawInputModifiers.HasFlag(RawInputModifiers.Control))
                    Input!(new RawTextInputEventArgs(_myKeyboardDevice,
                        timeStamp,
                        _inputRoot,
                        keyChar.ToString()));
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
                    _accessKeysAlwaysOnDisposable?.Dispose();
                    Console.Resized -= OnConsoleOnResized;
                    Console.KeyEvent -= ConsoleOnKeyEvent;
                    Console.MouseEvent -= ConsoleOnMouseEvent;
                    Console.FocusEvent -= ConsoleOnFocusEvent;

                    if (Console is IDisposable disposableConsole)
                        disposableConsole.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        /// <summary>
        ///     This works by creating a Software cursor, aka a sprite that is drawn on top of the screen.
        ///     It draws the current cursor directly to the console buffer, but maintains a list of the pixels need to be redrawn
        ///     whenever the cursor moves.
        /// </summary>
        /// <param name="point"></param>
        private void RenderSoftwareCursor(Point point)
        {
            lock (PixelBuffer)
            {
                // we need to maintain the caret 
                PixelBufferCoordinate oldCaretPosition = Console.GetCaretPosition();
                bool hasCaret = false;
                for (int i = 0; i < PixelBuffer.Length; i++)
                    if (PixelBuffer[i].IsCaret)
                    {
                        hasCaret = true;
                        // hide the caret while drawing
                        Console.HideCaret();
                        break;
                    }

                // draw any pixels from the pixelbuffer that
                // need to be refreshed because the cursor has moved away
                foreach (Rect rect in _refreshRects)
                    for (ushort x = (ushort)rect.Left; x <= (ushort)rect.Right; x++)
                        for (ushort y = (ushort)rect.Top; y <= (ushort)rect.Bottom; y++)
                            if (x < PixelBuffer.Width && y < PixelBuffer.Height)
                            {
                                Pixel pixel = PixelBuffer[x, y];
                                Console.Print(new PixelBufferCoordinate(x, y),
                                    pixel.Background.Color,
                                    pixel.Foreground.Color,
                                    pixel.Foreground.Style,
                                    pixel.Foreground.Weight,
                                    pixel.Foreground.TextDecoration,
                                    pixel.Foreground.Symbol.Text);
                            }

                _refreshRects.Clear();

                var cursorPosition = new PixelBufferCoordinate((ushort)Math.Max(0, point.X), (ushort)point.Y);
                string cursorText = GetCursorText();

                if (!string.IsNullOrWhiteSpace(cursorText))
                {
                    int width = ControlUtils.MeasureText(cursorText);
                    if (width <= PixelBuffer.Width - cursorPosition.X)
                    {
                        // add the rect to the refresh list
                        // NOTE: we maintain a list because for when cursor moves faster then refresh
                        _refreshRects.Add(new Rect((ushort)Math.Max(0, (int)point.X - 1), point.Y, width, 1));

                        // get current pixel so we know the background color
                        Pixel currentPixel = PixelBuffer[(ushort)point.X, (ushort)point.Y];

                        // Calculate the inverse color
                        Color invertColor = Color.FromRgb((byte)(255 - currentPixel.Background.Color.R),
                            (byte)(255 - currentPixel.Background.Color.G),
                            (byte)(255 - currentPixel.Background.Color.B));

                        // draw the cursor directly to console.
                        Console.Print(new PixelBufferCoordinate((ushort)point.X, (ushort)point.Y),
                            currentPixel.Background.Color,
                            invertColor,
                            null,
                            null,
                            null,
                            cursorText);
                    }
                }

                // restore the caret position if it was visible
                if (hasCaret)
                {
                    Console.SetCaretPosition(oldCaretPosition);
                    Console.ShowCaret();
                }
            }
        }

        private string GetCursorText()
        {
            return _cursorType switch
            {
                StandardCursorType.Arrow => " ",
                StandardCursorType.Cross => "+",
                StandardCursorType.Hand => "👆",
                StandardCursorType.Help => "?",
                StandardCursorType.No => "🚫",
                StandardCursorType.SizeAll => "*",
                StandardCursorType.SizeNorthSouth => "⬍",
                StandardCursorType.SizeWestEast => "⬌",
                StandardCursorType.Wait => "⧖",
                StandardCursorType.Ibeam => "I",
                StandardCursorType.UpArrow => "⬆",
                StandardCursorType.TopSide => "⬍", // "⬆",
                StandardCursorType.BottomSide => "⬍", // "⬇",
                StandardCursorType.LeftSide => "⬌", // "⬅",
                StandardCursorType.RightSide => "⬌", // "⮕",
                StandardCursorType.TopLeftCorner => "⤡", // "⬉",
                StandardCursorType.TopRightCorner => "⤢", // "⬈",
                StandardCursorType.BottomLeftCorner => "⤢", // "⬋",
                StandardCursorType.BottomRightCorner => "⤡", // "⬊",
                StandardCursorType.DragCopy => "+",
                StandardCursorType.DragLink => "⤻",
                StandardCursorType.DragMove => "◤",
                StandardCursorType.AppStarting => "⧖",
                StandardCursorType.None => " ",
                _ => " "
            };
        }
    }
}