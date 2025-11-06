using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Consolonia.Core.Controls
{
    internal static class ListBoxExtensions
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        internal static ListBoxItem? GetFocusedListBoxItem(this ListBox listBox)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            // 1) Check realized containers (fast, reliable when items are materialized)
            for (int i = 0; i < listBox.ItemCount; i++)
                if (listBox.ContainerFromIndex(i) is ListBoxItem container)
                    if (container.IsFocused || container.IsKeyboardFocusWithin)
                        return container;

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