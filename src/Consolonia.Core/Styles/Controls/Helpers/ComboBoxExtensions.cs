using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace Consolonia.Core.Styles.Controls.Helpers
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
                if ((bool)args.NewValue)
                    box.KeyDown += BoxOnKeyDown;
                else
                    box.KeyDown -= BoxOnKeyDown;
            });

            FocusOnOpenProperty.Changed.Subscribe(args =>
            {
                
                if (args.NewValue.Value)
                {
                    var itemsPresenter = (Visual)args.Sender;
                    itemsPresenter.AttachedToVisualTree += ItemContainerGeneratorOnMaterialized;
                    
                    static async void ItemContainerGeneratorOnMaterialized(object? sender,
                        VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
                    {
                        var comboBox = ((ItemsPresenter)sender).FindLogicalAncestorOfType<ComboBox>();
                        await Task.Yield();
                        typeof(ComboBox).GetMethod("TryFocusSelectedItem",
                                BindingFlags.Instance | BindingFlags.NonPublic)
                            .Invoke(comboBox, null);
                    }    
                }
                else
                {
                    throw new NotImplementedException();
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