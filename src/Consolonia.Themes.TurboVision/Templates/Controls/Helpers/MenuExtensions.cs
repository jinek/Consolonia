using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Consolonia.Core.Helpers;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    internal static class MenuExtensions
    {
        public static readonly AttachedProperty<bool> FocusOnLoadProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("FocusOnLoad", typeof(MenuExtensions));

        private static readonly AttachedProperty<IDisposable[]> DisposablesProperty =
            AvaloniaProperty.RegisterAttached<ItemsPresenter, IDisposable[]>("Disposables", typeof(MenuExtensions));

        static MenuExtensions()
        {
            FocusOnLoadProperty.Changed.SubscribeAction(args =>
            {
                var visual = (Visual)args.Sender;
                if (args.NewValue.Value)
                {
                    visual.AttachedToVisualTree += OnAttachedToVisualTree;
                    IDisposable disposable = visual
                        .GetPropertyChangedObservable(InputElement.IsKeyboardFocusWithinProperty)
                        .Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(eventArgs =>
                        {
                            if (!(bool)eventArgs.NewValue!)
                                Dispatcher.UIThread.Post(() =>
                                {
                                    var focusedControl = (Control)AvaloniaLocator.Current.GetRequiredService<IFocusManager>()!.GetFocusedElement()!;
                                    var menuItems = visual.GetLogicalAncestors().OfType<MenuItem>();

                                    var focusedTree = focusedControl.GetLogicalAncestors();

                                    foreach (MenuItem menuItem in menuItems.Where(item => !focusedTree.Contains(item))
                                                 .ToArray())
                                        menuItem.Close();
                                });
                        }));
                    visual.SetValue(DisposablesProperty, new[] { disposable });
                }
                else
                {
                    visual.AttachedToVisualTree -= OnAttachedToVisualTree;
                    foreach (IDisposable disposable in visual.GetValue(DisposablesProperty)) disposable.Dispose();
                }
            });
        }

        private static void OnAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var control = (Control)sender;
                control.FindDescendantOfType<MenuItem>()?.Focus();
            });
        }
    }
}