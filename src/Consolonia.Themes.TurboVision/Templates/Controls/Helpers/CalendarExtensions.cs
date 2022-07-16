using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public static class CalendarExtensions
    {
        public static readonly AttachedProperty<bool> ZoomOutOnKeyProperty =
            AvaloniaProperty.RegisterAttached<Calendar, bool>("ZoomOutOnKey", typeof(CalendarExtensions));

        static CalendarExtensions()
        {
            ZoomOutOnKeyProperty.Changed.Subscribe(args =>
            {
                var calendar = (Calendar)args.Sender;

                if (args.NewValue.GetValueOrDefault())
                    calendar.KeyDown += OnKeyDown;
                else
                    calendar.KeyDown -= OnKeyDown;
            });
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            var calendar = (Calendar)sender;
            if ((e.Key != Key.Back || e.KeyModifiers != KeyModifiers.None) && e.Key != Key.OemMinus) return;

            if (calendar.DisplayMode >= CalendarMode.Decade) return;
            calendar.DisplayMode++;
            e.Handled = true;
        }
    }
}