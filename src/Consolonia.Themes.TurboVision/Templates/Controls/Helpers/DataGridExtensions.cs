using Avalonia;
using Avalonia.Controls;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public static class DataGridExtensions
    {
        public static readonly AttachedProperty<bool> IsSelectedProperty =
            AvaloniaProperty.RegisterAttached<DataGridRow, bool>("IsSelected", typeof(DataGridExtensions));
    }
}