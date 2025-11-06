using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;

namespace Consolonia.Core.Controls
{
    public static class ListBoxExtensions
    {
        //public static object? GetFocusedListBoxItem(this ListBox listBox)
        //{
        //    // 1) Check realized containers (fast, reliable when items are materialized)
        //    for (int i = 0; i < listBox.ItemCount; i++)
        //    {
        //        if (listBox.ContainerFromIndex(i) is ListBoxItem container)
        //        {
        //            if (container.IsFocused || container.IsKeyboardFocusWithin)
        //                return listBox.ItemFromContainer(container);
        //        }
        //    }
        //    return null;
        //}

        public static ListBoxItem GetFocusedListBoxItem(this ListBox listBox)
        {
            // 1) Check realized containers (fast, reliable when items are materialized)
            for (int i = 0; i < listBox.ItemCount; i++)
            {
                if (listBox.ContainerFromIndex(i) is ListBoxItem container)
                {
                    if (container.IsFocused || container.IsKeyboardFocusWithin)
                        return container;
                }
            }
            return null;
        }

    }
}
