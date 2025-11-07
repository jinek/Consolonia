using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AvaloniaEdit;
using EditNET.DataModels;

namespace EditNET.Helpers
{
    public class SettingsToEditorOptionsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not Settings settings)
                return AvaloniaProperty.UnsetValue;
            return new TextEditorOptions
            {
                AcceptsTab = true,
                AllowToggleOverstrikeMode = true,
                EnableTextDragDrop = true,
                EnableRectangularSelection = false,
                ShowBoxForControlCharacters = true,
                EnableHyperlinks = true,
                EnableEmailHyperlinks = true,
                EnableVirtualSpace = false,
                HighlightCurrentLine = true,
                ShowSpaces = settings.ShowSpaces,
                ShowTabs = settings.ShowTabs
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}