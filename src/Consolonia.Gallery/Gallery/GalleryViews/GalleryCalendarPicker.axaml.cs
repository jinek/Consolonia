using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(35)]
    [UsedImplicitly]
    public class GalleryCalendarPicker : UserControl
    {
        public GalleryCalendarPicker()
        {
            InitializeComponent();
            DataContext = new GalleryCalendarPickerDataContext();

            var dp2 = this.Get<CalendarDatePicker>("DatePicker2");
            var dp3 = this.Get<CalendarDatePicker>("DatePicker3");
            var dp4 = this.Get<CalendarDatePicker>("DatePicker4");
            var dp5 = this.Get<CalendarDatePicker>("DatePicker5");

            dp2.SelectedDate = DateTime.Today.AddDays(10);
            dp3.SelectedDate = DateTime.Today.AddDays(20);
            dp5.SelectedDate = DateTime.Today;

            dp4.TemplateApplied += (_, _) => { dp4.BlackoutDates?.AddDatesInPast(); };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public sealed class GalleryCalendarPickerDataContext : INotifyPropertyChanged
    {
        private DateTime? _validatedDateExample;

        /// <summary>
        ///     A required DateTime which should demonstrate validation for the DateTimePicker
        /// </summary>
        [Required]
        public DateTime? ValidatedDateExample
        {
            get => _validatedDateExample;
            set
            {
                if (_validatedDateExample == value) return;
                _validatedDateExample = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}