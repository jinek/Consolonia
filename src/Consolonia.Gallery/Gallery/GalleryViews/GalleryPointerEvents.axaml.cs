using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Consolonia.Controls;
using YamlConverter;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class PointerEventViewModel
    {
        public PointerEventViewModel(PointerEventArgs e, PointerPoint point)
        {
            Name = $"[{point.Position.X},{point.Position.Y}] " + e switch
                {
                    PointerReleasedEventArgs a => "PointerReleased",
                    PointerPressedEventArgs a => "PointerPressed",
                    PointerWheelEventArgs a => "PointerWheelChanged",
                    PointerEventArgs a => "PointerMoved"
                } + $" {e.KeyModifiers.ToString()}";
            Point = point;
            Event = e;
            Details = YamlConvert.SerializeObject(Point.Properties);
            Details += $"Position: [{Point.Position}]\n";
            Details += $"KeyModifiers: {Event.KeyModifiers}\n";
            Details += $"WheelDelta: {(Event is PointerWheelEventArgs pwe ? pwe.Delta : 0)}\n";
            Details += $"ClickCount: {(Event is PointerPressedEventArgs ppe ? ppe.ClickCount : 0)}\n";
            Details +=
                $"InitialPressMouseButton: {(Event is PointerReleasedEventArgs pre ? pre.InitialPressMouseButton : MouseButton.None)}\n";
        }

        public string Name { get; set; }
        public PointerPoint Point { get; set; }
        public PointerEventArgs Event { get; set; }

        public string Details { get; set; }
    }

    public partial class GalleryPointerEvents : UserControl
    {
        private readonly ObservableCollection<PointerEventViewModel> _events = new();

        public GalleryPointerEvents()
        {
            InitializeComponent();

            DataContext = _events;
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            AddEvent(e, e.GetCurrentPoint(this));
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            AddEvent(e, e.GetCurrentPoint(this));
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            AddEvent(e, e.GetCurrentPoint(this));
        }

        private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
        {
            AddEvent(e, e.GetCurrentPoint(this));
        }

        private void AddEvent(PointerEventArgs e, PointerPoint point, [CallerMemberName] string? title = null)
        {
            var eventViewModel = new PointerEventViewModel(e, point);
            _events.Add(eventViewModel);
            if (_events.Count > 1000)
                _events.RemoveAt(0);
            Events.SelectedItem = eventViewModel;
        }


        private async void ListBox_DoubleTapped(object? sender, TappedEventArgs e)
        {
            var pev = Events.SelectedItem as PointerEventViewModel;
            await new MessageBox().ShowDialogAsync(this, pev.Details, pev.Name);
        }
    }
}