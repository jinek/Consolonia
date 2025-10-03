using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    /// <summary>
    ///     Converts a Size-like object to its Width (double).
    ///     Supports Avalonia.Size, System.Drawing.Size/SizeF, Consolonia.GuiCS.Size (int), and any object exposing a numeric
    ///     Width property.
    /// </summary>
    public sealed class SizeToWidthConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new SizeToWidthConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Size avaloniaSize && avaloniaSize.Width > 0)
                return avaloniaSize.Width;

            // fallback
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("SizeToWidthConverter does not support ConvertBack.");
        }
    }

    /// <summary>
    ///     Converts a Size-like object to its Height (double).
    ///     Supports Avalonia.Size, System.Drawing.Size/SizeF, Consolonia.GuiCS.Size (int), and any object exposing a numeric
    ///     Height property.
    /// </summary>
    public sealed class SizeToHeightConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new SizeToHeightConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Size avaloniaSize && avaloniaSize.Height > 0)
                return avaloniaSize.Height;

            // fallback
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("SizeToHeightConverter does not support ConvertBack.");
        }
    }
}