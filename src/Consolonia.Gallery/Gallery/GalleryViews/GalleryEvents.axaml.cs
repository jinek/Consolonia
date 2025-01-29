using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Controls;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Gallery.Gallery.GalleryViews
{

    public partial class GalleryEvents : UserControl
    {
        private IConsole _console;

        public GalleryEvents()
        {
            InitializeComponent();

            this.DataContext = new EventsViewModel();
        }

        public EventsViewModel ViewModel => this.DataContext as EventsViewModel;

        private void OnRawMouse(RawPointerEventType type, Point point, Vector? nullable, RawInputModifiers modifiers)
        {
            var eventViewModel = new RawMouseEventViewModel(type, point, nullable, modifiers);
            ViewModel.RawMouseEvents.Add(eventViewModel);
            ViewModel.SelectedRawMouseEvent = eventViewModel;
        }

        private void OnRawKey(Key arg1, char arg2, RawInputModifiers arg3, bool arg4, ulong arg5)
        {
            var eventViewModel = new RawKeyboardEventViewModel(arg1, arg2, arg3, arg4, arg5);
            ViewModel.RawKeyboardEvents.Add(eventViewModel);
            ViewModel.SelectedRawKeyboardEvent = eventViewModel;
        }


        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            var eventViewModel = new KeyEventViewModel(e);
            ViewModel.KeyboardEvents.Add(eventViewModel);
            ViewModel.SelectedKeyboardEvent = eventViewModel;
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            var eventViewModel = new KeyEventViewModel(e);
            ViewModel.KeyboardEvents.Add(eventViewModel);
            ViewModel.SelectedKeyboardEvent = eventViewModel;
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }

        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }


        private async void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (sender is ListBox lb)
            {
                var vm = lb.SelectedItem as EventViewModel;
                await new MessageBox().ShowDialogAsync(this, vm.Details, vm.Name);
            }
        }


        private void OnRawMouseEntered(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            EnsureConsole();
            _console.MouseEvent += OnRawMouse;
        }

        private void OnRawMouseExited(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            EnsureConsole();
            _console.MouseEvent -= OnRawMouse;
        }

        private void OnRawKeyboardGotFocus(object? sender, Avalonia.Input.GotFocusEventArgs e)
        {
            EnsureConsole();
            _console.KeyEvent += OnRawKey;
        }

        private void OnRawKeyboardLostFocus(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            EnsureConsole();
            _console.KeyEvent -= OnRawKey;
        }

        private void EnsureConsole()
        {
            // we reach into internal Console property because we want to hook raw console events
            if (_console == null)
            {
                var consoleWindow = (ConsoleWindow)TopLevel.GetTopLevel(this).PlatformImpl;
                var propInfo = typeof(ConsoleWindow).GetField("Console", BindingFlags.NonPublic | BindingFlags.Instance);
                _console = (IConsole)propInfo.GetValue(consoleWindow);
            }
        }

    }

    public class EventViewModel
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Details { get; set; }
    }

    public class KeyEventViewModel : EventViewModel
    {
        public KeyEventViewModel(KeyEventArgs e, [CallerMemberName] string? name = null)
        {
            Name = name;
            Summary = $"{name} {e.Key} ({e.KeyModifiers})";
            Details = $"""
                PhysicalKey: {e.PhysicalKey}
                Key: {e.Key} {(int)e.Key}
                KeyDeviceType: {e.KeyDeviceType}
                KeyModifiers: {e.KeyModifiers}
                KeySymbol: {e.KeySymbol}
                """;
        }
    }

    public class PointerEventViewModel : EventViewModel
    {
        public PointerEventViewModel(PointerEventArgs e, PointerPoint point, [CallerMemberName] string? name = null)
        {
            Name = name;
            Summary = $"[{point.Position.X},{point.Position.Y}] {name} ({e.KeyModifiers.ToString()})";
            Details =
                $"""
                Position: [{point.Position}]
                KeyModifiers: {e.KeyModifiers}
                WheelData: {(e is PointerWheelEventArgs pwe ? pwe.Delta : 0)}
                ClickCount: {(e is PointerPressedEventArgs ppe ? ppe.ClickCount : 0)}
                Pointer.Type: {point.Pointer.Type}
                Pointer.Id: {point.Pointer.Id}
                Pointer.IsPrimary: {point.Pointer.IsPrimary}
                Pointer.Captured: {point.Pointer.Captured}
                InitialPressMouseButton: {(e is PointerReleasedEventArgs pre ? pre.InitialPressMouseButton : MouseButton.None)}
                """;
        }
    }

    public class RawMouseEventViewModel : EventViewModel
    {
        public RawMouseEventViewModel(RawPointerEventType type, Point point, Vector? nullable, RawInputModifiers modifiers)
        {
            Name = type.ToString(); 
            Summary = $"[{point}] {type} ({modifiers.ToString()})";
            Details = $"""
                Point: {point}
                Modifiers: {modifiers.ToString()}
                Vector: {nullable}
                """;
        }
    }

    public class RawKeyboardEventViewModel : EventViewModel
    {
        public RawKeyboardEventViewModel(Key key, char ch, RawInputModifiers modifiers, bool isDown, ulong timestamp)
        {
            Name = $"RawKey{(isDown ? "Down" : "Up")} {key} ({modifiers})";
            Summary = Name;
            Details = 
                $"""
                Key: {key.ToString()}
                Char: '{ch}' {(int)ch} 0x{((int)ch).ToString("x")}
                Modifiers: {modifiers.ToString()}
                IsDown: {isDown}
                Timestamp: {timestamp}
                """;
        }
    }

    public partial class EventsViewModel : ObservableObject
    {
        public EventsViewModel()
        {
            _keyboardEvents.CollectionChanged += (sender, args) =>
            {
                while (_keyboardEvents.Count > 1000)
                    _keyboardEvents.RemoveAt(0);
            };
            _pointerEvents.CollectionChanged += (sender, args) =>
            {
                while (_pointerEvents.Count > 1000)
                    _pointerEvents.RemoveAt(0);
            };
        }

        [ObservableProperty]
        private ObservableCollection<EventViewModel> _keyboardEvents = new ObservableCollection<EventViewModel>();

        [ObservableProperty]
        private EventViewModel _selectedKeyboardEvent;

        [ObservableProperty]
        private ObservableCollection<EventViewModel> _pointerEvents = new ObservableCollection<EventViewModel>();

        [ObservableProperty]
        private EventViewModel _selectedPointerEvent;

        [ObservableProperty]
        private ObservableCollection<RawMouseEventViewModel> _rawMouseEvents = new ObservableCollection<RawMouseEventViewModel>();

        [ObservableProperty]
        private EventViewModel _selectedRawMouseEvent;

        [ObservableProperty]
        private ObservableCollection<RawKeyboardEventViewModel> _rawKeyboardEvents = new ObservableCollection<RawKeyboardEventViewModel>();

        [ObservableProperty]
        private EventViewModel _SelectedRawKeyboardEvent;
    }

}