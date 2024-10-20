using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    internal static class DropDownExtensions
    {
        private static readonly AttachedProperty<IDisposable[]> DisposablesProperty =
            AvaloniaProperty.RegisterAttached<Control, IDisposable[]>("Disposables", typeof(DropDownExtensions));

        public static void ProcessFocusOnOpen<TElementType, TParentControl>(AvaloniaPropertyChangedEventArgs<bool> args,
            AvaloniaProperty<bool> dropDownProperty, EventHandler<VisualTreeAttachmentEventArgs> focusDropDownAction,
            Action<TParentControl> focusParentAction)
            where TElementType : Control where TParentControl : Control
        {
            var dropDownControl = (TElementType)args.Sender;
            var parentControl = dropDownControl.FindLogicalAncestorOfType<TParentControl>()!;

            if (args.NewValue.Value)
            {
                dropDownControl.AttachedToVisualTree += focusDropDownAction;

                IDisposable disposable1 = parentControl.GetPropertyChangedObservable(InputElement.IsFocusedProperty)
                    .Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(eventArgs =>
                    {
                        if (!(bool)eventArgs.NewValue! && !dropDownControl.IsKeyboardFocusWithin)
                            Dispatcher.UIThread.Post(() => { parentControl.SetValue(dropDownProperty, false); });
                    }));

                IDisposable disposable2 = dropDownControl
                    .GetPropertyChangedObservable(InputElement.IsKeyboardFocusWithinProperty)
                    .Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(eventArgs =>
                    {
                        if (!(bool)eventArgs.NewValue! && !parentControl.IsKeyboardFocusWithin)
                            Dispatcher.UIThread.Post(() =>
                            {
                                parentControl.SetValue(dropDownProperty, false);
                                focusParentAction(parentControl);
                            });
                    }));

                dropDownControl.SetValue(DisposablesProperty, new[] { disposable1, disposable2 });
            }
            else
            {
                var disposables = dropDownControl.GetValue(DisposablesProperty);
                foreach (IDisposable disposable in disposables)
                    disposable.Dispose();


                dropDownControl.AttachedToVisualTree -= focusDropDownAction;
            }
        }
    }
}