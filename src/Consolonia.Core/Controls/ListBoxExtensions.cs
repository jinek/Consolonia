using System;
using System.Collections.Specialized;
using System.Threading;
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
