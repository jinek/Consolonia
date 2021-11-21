using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    internal static class ComboBoxExtensions
    {
        public static readonly AttachedProperty<bool> OpenOnEnterProperty =
            AvaloniaProperty.RegisterAttached<ComboBox, bool>("OpenOnEnter", typeof(ComboBoxExtensions));

        public static readonly AttachedProperty<bool> FocusOnOpenProperty =
            AvaloniaProperty.RegisterAttached<ItemsPresenter, bool>("FocusOnOpen", typeof(ComboBoxExtensions));

        private static readonly AttachedProperty<IDisposable[]> DisposablesProperty =
            AvaloniaProperty.RegisterAttached<ItemsPresenter, IDisposable[]>("Disposables", typeof(ComboBoxExtensions));

        static ComboBoxExtensions()
        {
            OpenOnEnterProperty.Changed.AddClassHandler<ComboBox>((box, args) =>
            {
                if ((bool)args.NewValue)
                    box.KeyDown += BoxOnKeyDown;
                else
                    box.KeyDown -= BoxOnKeyDown;
            });

            FocusOnOpenProperty.Changed.Subscribe(args =>
            {
                var itemsPresenter = (ItemsPresenter)args.Sender;
                var comboBox = itemsPresenter.FindLogicalAncestorOfType<ComboBox>();

                if (args.NewValue.Value)
                {
                    itemsPresenter.AttachedToVisualTree += ItemContainerGeneratorOnMaterialized;

                    IDisposable disposable1 = comboBox.GetPropertyChangedObservable(ComboBox.IsFocusedProperty)
                        .Subscribe(eventArgs =>
                        {
                            if (!(bool)eventArgs.NewValue && !itemsPresenter.IsKeyboardFocusWithin)
                            {
                                Dispatcher.UIThread.Post(() => { comboBox.IsDropDownOpen = false; });
                            }
                        });

                    IDisposable disposable2 = itemsPresenter
                        .GetPropertyChangedObservable(ItemsPresenter.IsKeyboardFocusWithinProperty)
                        .Subscribe(eventArgs =>
                        {
                            if (!(bool)eventArgs.NewValue && !comboBox.IsKeyboardFocusWithin)
                            {
                                Dispatcher.UIThread.Post(() =>
                                {
                                    comboBox.IsDropDownOpen = false;
                                    comboBox.Focus();
                                });
                            }
                        });

                    itemsPresenter.SetValue(DisposablesProperty, new[] { disposable1, disposable2 });
                }
                else
                {
                    var disposables = itemsPresenter.GetValue(DisposablesProperty);
                    foreach (IDisposable disposable in disposables)
                        disposable.Dispose();

                    itemsPresenter.AttachedToVisualTree -= ItemContainerGeneratorOnMaterialized;
                }

                static void ItemContainerGeneratorOnMaterialized(object? sender,
                    VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
                {
                    var comboBox = ((ItemsPresenter)sender).FindLogicalAncestorOfType<ComboBox>();
                    Dispatcher.UIThread.Post(() =>
                    {
                        typeof(ComboBox).GetMethod("TryFocusSelectedItem",
                                BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(comboBox, null);
                    });
                }
            });

            FocusOnOpenProperty.Changed.AddClassHandler<ComboBox>((box, args) =>
            {
                if ((bool)args.NewValue)
                    box.KeyDown += BoxOnKeyDown;
                else
                    box.KeyDown -= BoxOnKeyDown;
            });
        }

        private static void BoxOnKeyDown(object sender, KeyEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (e.Key != Key.Enter || comboBox.IsDropDownOpen) return;
            comboBox.IsDropDownOpen = true;
            e.Handled = true;
        }
    }
}