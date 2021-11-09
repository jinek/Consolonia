using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    internal static class ComboBoxExtensions
    {
        public static readonly AttachedProperty<bool> OpenOnEnterProperty =
            AvaloniaProperty.RegisterAttached<ComboBox, bool>("OpenOnEnter", typeof(ComboBoxExtensions));
        
        public static readonly AttachedProperty<bool> FocusOnOpenProperty =
            AvaloniaProperty.RegisterAttached<ComboBox, bool>("FocusOnOpen", typeof(ComboBoxExtensions));

        static ComboBoxExtensions()
        {
            OpenOnEnterProperty.Changed.AddClassHandler<ComboBox>((box, args) =>
            {
                if ((bool)args.NewValue)
                    box.KeyDown += BoxOnKeyDown;
                else
                    box.KeyDown -= BoxOnKeyDown;
            });

            /* todo: this does not work
             ComboBox.IsDropDownOpenProperty.Changed.Subscribe(Observer.Create<AvaloniaPropertyChangedEventArgs>(args =>
            {
                if (args.Sender.GetValue(FocusOnOpenProperty))
                {
                    var comboBox = (ComboBox)args.Sender;
                    ItemsPresenter? PART_ItemsPresenter = null;
                    foreach (IControl templateChild in comboBox.GetTemplateChildren())
                    {
                        var itemsPresenter = templateChild.FindLogicalDescendantOfType<ItemsPresenter>();
                        if (itemsPresenter?.Name == "PART_ItemsPresenter")
                        {
                            PART_ItemsPresenter = itemsPresenter;
                            break;
                        }
                    }

                    if (PART_ItemsPresenter == null)
                        throw new NotImplementedException();

                    Dispatcher.UIThread.Post(() => {
                    ComboBoxItem? comboBoxItem = PART_ItemsPresenter.Items.Cast<ComboBoxItem>().ToArray()[comboBox.SelectedIndex];
                        
                    comboBoxItem?.Focus();
                    });
                }
            }));*/
            
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