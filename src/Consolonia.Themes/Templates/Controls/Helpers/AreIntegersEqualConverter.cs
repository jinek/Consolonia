using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Consolonia.Themes.Templates.Controls.Helpers
{
    public class AreIntegersEqualConverter : IMultiValueConverter
    {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            int[] ints = values.Cast<int>().ToArray();
            int first = ints.FirstOrDefault();

            for (int i = 1; i < values.Count; i++)
                if (ints[i] != first)
                    return false;

            return true;
        }
    }
}