using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class MathSubtractConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dv && parameter is double dp)
            {
                return dv - dp;
            }
            return double.NaN;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}