using Avalonia;
using Avalonia.Controls;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    internal static class DataGridExtensions
    {
        public static readonly AttachedProperty<bool> IsSelectedProperty =
            AvaloniaProperty.RegisterAttached<DataGridRow, bool>("IsSelected", typeof(DataGridExtensions));
    }
}