using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Threading;

namespace Consolonia.Core.Controls
{
    internal static class ListBoxExtensions
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

        internal static ListBoxItem GetFocusedListBoxItem(this ListBox listBox)
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

        internal static void KeepFocus(this ListBox listBox, Func<bool> keepFocus)
        {
            listBox.Items.CollectionChanged += (_, _) =>
            {
                if (!keepFocus())
                    return;
                Dispatcher.UIThread.Post(() =>
                {
                    if (listBox.ItemCount > 0)
                    {
                        var firstItemContainer = listBox.ContainerFromIndex(0) as ListBoxItem;
                        firstItemContainer?.Focus();
                    }
                }, DispatcherPriority.Background);
            };
        }

    }
}
