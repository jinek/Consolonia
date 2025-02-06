using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();
            if (str == null)
                return AvaloniaProperty.UnsetValue;
            if (int.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int x))
                return (decimal)x;
            return AvaloniaProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal d)
                return ((int)d).ToString("X8");
            return AvaloniaProperty.UnsetValue;
        }
    }
}