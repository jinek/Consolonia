using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Consolonia.Controls;
using YamlConverter;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class KeyEventViewModel
    {
        public KeyEventViewModel(KeyEventArgs e, string name)
        {
            Name = $"{name} {e.KeyModifiers} {e.Key}";
            Event = e;
            Details = YamlConvert.SerializeObject(new
            {
                PhysicalKey = $"{(int)e.PhysicalKey} {e.PhysicalKey}",
                Key = $"{(int)e.Key} {e.Key}",
                KeyDeviceType = e.KeyDeviceType.ToString(),
                KeyModifiers = e.KeyModifiers.ToString(),
                KeySymbol = e.KeySymbol?.ToString()
            });
        }

        public string Name { get; set; }
        public KeyEventArgs Event { get; set; }

        public string Details { get; set; }
    }

    public partial class GalleryKeyboardEvents : UserControl
    {

        private ObservableCollection<KeyEventViewModel> _events = new ObservableCollection<KeyEventViewModel>();

        public GalleryKeyboardEvents()
        {
            InitializeComponent();

            this.DataContext = _events;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            AddEvent(e);
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            AddEvent(e);
        }


        private void AddEvent(KeyEventArgs e, [CallerMemberName] string? title = null)
        {
            var eventViewModel = new KeyEventViewModel(e, title);
            _events.Add(eventViewModel);
            if (_events.Count > 1000)
                _events.RemoveAt(0);
            this.Events.SelectedItem = eventViewModel;
        }


        private async void ListBox_DoubleTapped(object? sender, TappedEventArgs e)
        {
            var pev = this.Events.SelectedItem as KeyEventViewModel;
            await new MessageBox().ShowDialogAsync(this, pev.Details, pev.Name);
        }
    }
}