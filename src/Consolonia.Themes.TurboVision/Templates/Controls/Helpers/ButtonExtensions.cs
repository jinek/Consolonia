using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Reactive;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public static class ButtonExtensions
    {
        //todo: names don't match
        public static readonly AttachedProperty<TimeSpan> DelayPressProperty =
            AvaloniaProperty.RegisterAttached<Button, TimeSpan>("IsDelayPress", typeof(ButtonExtensions));

        public static readonly AttachedProperty<bool> ShadowProperty =
            AvaloniaProperty.RegisterAttached<Button, bool>("ShadowPress", typeof(ButtonExtensions));

        static ButtonExtensions()
        {
            Button.ClickEvent.Raised.Subscribe(new AnonymousObserver<(object, RoutedEventArgs)>(async tuple =>
            {
                (object sender, RoutedEventArgs e) = tuple;
                if (sender != e.Source) return;
                var button = (Button)sender;

                TimeSpan timeout = button.GetValue(DelayPressProperty);
                if (timeout == DelayPressProperty.GetDefaultValue(typeof(Button)))
                    return;

                PseudolassesExtensions.Set(button.Classes, ":clickdelayed", true);

                await Task.Delay(timeout).ConfigureAwait(true); //todo: magic number

                PseudolassesExtensions.Set(button.Classes, ":clickdelayed", false);
            }));
        }
    }
}