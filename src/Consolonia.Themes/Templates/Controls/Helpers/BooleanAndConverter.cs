using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    public class BooleanAndConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            bool convert = values.All(val => (bool)val);
            return convert;
        }
    }
}