using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
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

            DataContext = new EventsViewModel();
        }

        public EventsViewModel ViewModel => DataContext as EventsViewModel;

        private void OnRawMouse(RawPointerEventType type, Point point, Vector? nullable, RawInputModifiers modifiers)
        {
            var eventViewModel = new RawMouseEventViewModel(type, point, nullable, modifiers);
            ViewModel.RawMouseEvents.Add(eventViewModel);
            ViewModel.SelectedRawMouseEvent = eventViewModel;
        }

        private void OnRawKey(Key arg1, char arg2, RawInputModifiers arg3, bool arg4, ulong arg5, bool arg6)
        {
            var eventViewModel = new RawKeyboardEventViewModel(arg1, arg2, arg3, arg4, arg5, arg6);
            ViewModel.RawKeyboardEvents.Add(eventViewModel);
            ViewModel.SelectedRawKeyboardEvent = eventViewModel;
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var eventViewModel = new KeyEventViewModel(e);
            ViewModel.KeyboardEvents.Add(eventViewModel);
            ViewModel.SelectedKeyboardEvent = eventViewModel;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            var eventViewModel = new KeyEventViewModel(e);
            ViewModel.KeyboardEvents.Add(eventViewModel);
            ViewModel.SelectedKeyboardEvent = eventViewModel;
        }

        private void OnPointerMoved(object sender, PointerEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }

        private void OnPointerPressed(object sender, PointerPressedEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }

        private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }

        private void OnPointerWheelChanged(object sender, PointerWheelEventArgs e)
        {
            var eventViewModel = new PointerEventViewModel(e, e.GetCurrentPoint(this));
            ViewModel.PointerEvents.Add(eventViewModel);
            ViewModel.SelectedPointerEvent = eventViewModel;
        }


        private async void OnDoubleTapped(object sender, TappedEventArgs e)
        {
            if (sender is ListBox lb)
            {
                var vm = lb.SelectedItem as EventViewModel;
                await MessageBox.ShowDialog(this, vm.Details, vm.Name);
            }
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            if (_console != null)
            {
                _console.KeyEvent -= OnRawKey;
                _console.MouseEvent -= OnRawMouse;
            }
        }

        private void OnRawMouseEntered(object sender, PointerEventArgs e)
        {
            EnsureConsole();
            _console.MouseEvent += OnRawMouse;
        }

        private void OnRawMouseExited(object sender, PointerEventArgs e)
        {
            EnsureConsole();
            _console.MouseEvent -= OnRawMouse;
        }

        private void OnRawKeyboardGotFocus(object sender, GotFocusEventArgs e)
        {
            EnsureConsole();
            _console.KeyEvent += OnRawKey;
        }

        private void OnRawKeyboardLostFocus(object sender, RoutedEventArgs e)
        {
            EnsureConsole();
            _console.KeyEvent -= OnRawKey;
        }

        private void EnsureConsole()
        {
            // we reach into internal Console property because we want to hook raw console events
            if (_console == null)
            {
                var consoleTopLevelInfo = (ConsoleWindowImpl)TopLevel.GetTopLevel(this).PlatformImpl;
                FieldInfo propInfo =
                    typeof(ConsoleWindowImpl).GetField("Console", BindingFlags.NonPublic | BindingFlags.Instance);
                _console = (IConsole)propInfo.GetValue(consoleTopLevelInfo);
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
        public KeyEventViewModel(KeyEventArgs e, [CallerMemberName] string name = null)
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
        public PointerEventViewModel(PointerEventArgs e, PointerPoint point, [CallerMemberName] string name = null)
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
        public RawMouseEventViewModel(RawPointerEventType type, Point point, Vector? nullable,
            RawInputModifiers modifiers)
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
        public RawKeyboardEventViewModel(Key key, char ch, RawInputModifiers modifiers, bool isDown, ulong timestamp,
            bool tryAsTextInput)
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
                 TryAsTextInput: {tryAsTextInput}
                 """;
        }
    }

    public partial class EventsViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<EventViewModel> _keyboardEvents = new();

        [ObservableProperty] private ObservableCollection<EventViewModel> _pointerEvents = new();

        [ObservableProperty] private ObservableCollection<RawKeyboardEventViewModel> _rawKeyboardEvents = new();

        [ObservableProperty] private ObservableCollection<RawMouseEventViewModel> _rawMouseEvents = new();

        [ObservableProperty] private EventViewModel _selectedKeyboardEvent;

        [ObservableProperty] private EventViewModel _selectedPointerEvent;

        [ObservableProperty] private EventViewModel _selectedRawKeyboardEvent;

        [ObservableProperty] private EventViewModel _selectedRawMouseEvent;

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
            _rawMouseEvents.CollectionChanged += (sender, args) =>
            {
                while (_rawMouseEvents.Count > 1000)
                    _rawMouseEvents.RemoveAt(0);
            };
            _rawKeyboardEvents.CollectionChanged += (sender, args) =>
            {
                while (_rawKeyboardEvents.Count > 1000)
                    _rawKeyboardEvents.RemoveAt(0);
            };
        }
    }
}