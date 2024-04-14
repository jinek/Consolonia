using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Consolonia.Core.Helpers;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    internal static class CalendarDatePickerExtensions
    {
        public static readonly AttachedProperty<bool> OpenOnEnterProperty =
            AvaloniaProperty.RegisterAttached<CalendarDatePicker, bool>("OpenOnEnter",
                typeof(CalendarDatePickerExtensions));

        public static readonly AttachedProperty<bool> FocusOnOpenProperty =
            AvaloniaProperty.RegisterAttached<Calendar, bool>("FocusOnOpen", typeof(CalendarDatePickerExtensions));

        static CalendarDatePickerExtensions()
        {
            OpenOnEnterProperty.Changed.AddClassHandler<CalendarDatePicker>((calendarDatePicker, args) =>
            {
                if ((bool)args.NewValue!)
                    calendarDatePicker.KeyUp += CalendarOnKeyUp;
                else
                    calendarDatePicker.KeyUp -= CalendarOnKeyUp;
            });

            FocusOnOpenProperty.Changed.SubscribeAction(args =>
            {
                DropDownExtensions.ProcessFocusOnOpen<Calendar, CalendarDatePicker>(args,
                    CalendarDatePicker.IsDropDownOpenProperty,
                    FocusOnDropDown, async calendarDatePicker =>
                    {
                        calendarDatePicker.Focus();
                        var textBox =
                            calendarDatePicker
                                .FindDescendantOfType<TextBox>(); // todo: need to find by name in template
                        await Task.Yield();
                        Dispatcher.UIThread.Post(() => { textBox?.Focus(); });
                    });

                static void FocusOnDropDown(object sender,
                    VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
                {
                    var calendar = (Calendar)sender;
                    Dispatcher.UIThread.Post(() => { calendar.Focus(); });
                }
            });
        }

        private static void CalendarOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Source is not TextBox) return;
            var calendarDatePicker = (CalendarDatePicker)sender;
            if (e.Key != Key.Enter || calendarDatePicker.IsDropDownOpen) return;
            calendarDatePicker.IsDropDownOpen = true;
            e.Handled = true;
        }
    }
}