using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    public class ValueToLeftPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                // Convert the double value to a left padding value
                // For example, you can return a new Thickness with the double value as the left padding
                return new Avalonia.Thickness(doubleValue, 0, 0, 0);
            }

            throw new ArgumentException("Value must be a double", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}