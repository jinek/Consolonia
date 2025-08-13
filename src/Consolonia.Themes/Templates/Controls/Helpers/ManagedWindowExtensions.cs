using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Consolonia.Controls;
using Iciclecreek.Avalonia.WindowManager;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    public sealed class ManagedWindowExtensions
    {
        static ManagedWindowExtensions()
        {
            CloseWindowGestureProperty.Changed.SubscribeAction(args =>
            {
                SetupNewKeyBinding(args, window => window.CloseCommand);
            });
            MaximizeWindowGestureProperty.Changed.SubscribeAction(args =>
            {
                SetupNewKeyBinding(args, window => window.MaximizeCommand);
            });
            MinimizeWindowGestureProperty.Changed.SubscribeAction(args =>
            {
                SetupNewKeyBinding(args, window => window.MinimizeCommand);
            });
            MoveWindowGestureProperty.Changed.SubscribeAction(args =>
            {
                SetupNewKeyBinding(args, window => window.MoveCommand);
            });
            SizeWindowGestureProperty.Changed.SubscribeAction(args =>
            {
                SetupNewKeyBinding(args, window => window.SizeCommand);
            });
            RestoreWindowGestureProperty.Changed.SubscribeAction(args =>
            {
                SetupNewKeyBinding(args, window => window.RestoreCommand);
            });
        }

        private static void SetupNewKeyBinding(AvaloniaPropertyChangedEventArgs<KeyGesture> args,
            Func<ManagedWindow, ICommand> commandAccessor)
        {
            var managedWindow = (ManagedWindow)args.Sender;
            KeyGesture gesture = args.NewValue.GetValueOrDefault();
            ICommand command = commandAccessor(managedWindow);
            KeyBinding existingCommand = managedWindow.KeyBindings.LastOrDefault(binding => binding.Command == command);
            if (existingCommand != null) managedWindow.KeyBindings.Remove(existingCommand);
            managedWindow.KeyBindings.Add(new KeyBinding
            {
                Command = command,
                Gesture = gesture
            });
        }

        public static readonly AttachedProperty<KeyGesture> CloseWindowGestureProperty =
            AvaloniaProperty.RegisterAttached<ManagedWindowExtensions, ManagedWindow, KeyGesture>(
                ControlUtils.GetStyledPropertyName());
        public static readonly AttachedProperty<KeyGesture> MaximizeWindowGestureProperty =
            AvaloniaProperty.RegisterAttached<ManagedWindowExtensions, ManagedWindow, KeyGesture>(
                ControlUtils.GetStyledPropertyName());
        public static readonly AttachedProperty<KeyGesture> MinimizeWindowGestureProperty =
            AvaloniaProperty.RegisterAttached<ManagedWindowExtensions, ManagedWindow, KeyGesture>(
                ControlUtils.GetStyledPropertyName());
        public static readonly AttachedProperty<KeyGesture> MoveWindowGestureProperty =
            AvaloniaProperty.RegisterAttached<ManagedWindowExtensions, ManagedWindow, KeyGesture>(
                ControlUtils.GetStyledPropertyName());
        public static readonly AttachedProperty<KeyGesture> SizeWindowGestureProperty =
            AvaloniaProperty.RegisterAttached<ManagedWindowExtensions, ManagedWindow, KeyGesture>(
                ControlUtils.GetStyledPropertyName());
        public static readonly AttachedProperty<KeyGesture> RestoreWindowGestureProperty =
            AvaloniaProperty.RegisterAttached<ManagedWindowExtensions, ManagedWindow, KeyGesture>(
                ControlUtils.GetStyledPropertyName());

        public static void SetCloseWindowGesture(ManagedWindow obj, KeyGesture value) =>
            obj.SetValue(CloseWindowGestureProperty, value);
        public static KeyGesture GetCloseWindowGesture(ManagedWindow obj) => obj.GetValue(CloseWindowGestureProperty);

        public static void SetMaximizeWindowGesture(ManagedWindow obj, KeyGesture value) =>
            obj.SetValue(MaximizeWindowGestureProperty, value);
        public static KeyGesture GetMaximizeWindowGesture(ManagedWindow obj) => obj.GetValue(MaximizeWindowGestureProperty);

        public static void SetMinimizeWindowGesture(ManagedWindow obj, KeyGesture value) =>
            obj.SetValue(MinimizeWindowGestureProperty, value);
        public static KeyGesture GetMinimizeWindowGesture(ManagedWindow obj) => obj.GetValue(MinimizeWindowGestureProperty);

        public static void SetMoveWindowGesture(ManagedWindow obj, KeyGesture value) =>
            obj.SetValue(MoveWindowGestureProperty, value);
        public static KeyGesture GetMoveWindowGesture(ManagedWindow obj) => obj.GetValue(MoveWindowGestureProperty);

        public static void SetSizeWindowGesture(ManagedWindow obj, KeyGesture value) =>
            obj.SetValue(SizeWindowGestureProperty, value);
        public static KeyGesture GetSizeWindowGesture(ManagedWindow obj) => obj.GetValue(SizeWindowGestureProperty);

        public static void SetRestoreWindowGesture(ManagedWindow obj, KeyGesture value) =>
            obj.SetValue(RestoreWindowGestureProperty, value);
        public static KeyGesture GetRestoreWindowGesture(ManagedWindow obj) => obj.GetValue(RestoreWindowGestureProperty);
    }
}