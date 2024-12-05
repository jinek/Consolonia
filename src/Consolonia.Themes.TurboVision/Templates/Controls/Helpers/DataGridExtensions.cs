using Avalonia;
using Avalonia.Controls;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public static class DataGridExtensions
    {
        public static readonly AttachedProperty<bool> IsSelectedProperty =
            AvaloniaProperty.RegisterAttached<DataGridRow, bool>("IsSelected", typeof(DataGridExtensions));
        
        public static bool GetIsSelected(DataGridRow element)
        {
            return element.GetValue(IsSelectedProperty);
        }
        
        public static void SetIsSelected(DataGridRow element, bool value)
        {
            element.SetValue(IsSelectedProperty, value);
        }
    }
}