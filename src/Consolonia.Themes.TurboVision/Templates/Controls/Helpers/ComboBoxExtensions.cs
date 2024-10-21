using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Consolonia.Core.Helpers;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    internal static class ComboBoxExtensions
    {
        public static readonly AttachedProperty<bool> OpenOnEnterProperty =
            AvaloniaProperty.RegisterAttached<ComboBox, bool>("OpenOnEnter", typeof(ComboBoxExtensions));

        public static readonly AttachedProperty<bool> FocusOnOpenProperty =
            AvaloniaProperty.RegisterAttached<ItemsPresenter, bool>("FocusOnOpen", typeof(ComboBoxExtensions));

        static ComboBoxExtensions()
        {
            OpenOnEnterProperty.Changed.AddClassHandler<ComboBox>((box, args) =>
            {
                if ((bool)args.NewValue!)
                    box.KeyDown += ComboBoxOnKeyDown;
                else
                    box.KeyDown -= ComboBoxOnKeyDown;
            });

            FocusOnOpenProperty.Changed.SubscribeAction(args =>
            {
                DropDownExtensions.ProcessFocusOnOpen<ItemsPresenter, ComboBox>(args, ComboBox.IsDropDownOpenProperty,
                    FocusOnDropDown,
                    comboBox => comboBox.Focus());

                static void FocusOnDropDown(object sender,
                    VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
                {
                    var comboBox = ((ItemsPresenter)sender).FindLogicalAncestorOfType<ComboBox>();
                    Dispatcher.UIThread.Post(() =>
                    {
                        typeof(ComboBox).GetMethod("TryFocusSelectedItem",
                                BindingFlags.Instance | BindingFlags.NonPublic)!
                            .Invoke(comboBox, null);
                    });
                }
            });

            FocusOnOpenProperty.Changed.AddClassHandler<ComboBox>((comboBox, args) =>
            {
                if ((bool)args.NewValue!)
                    comboBox.KeyDown += ComboBoxOnKeyDown;
                else
                    comboBox.KeyDown -= ComboBoxOnKeyDown;
            });
        }

        private static void ComboBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (e.Key != Key.Enter || comboBox.IsDropDownOpen) return;
            comboBox.IsDropDownOpen = true;
            e.Handled = true;
        }
    }
}