using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Data.Converters;

namespace Edit.NET
{
    public static class Converters
    {
        public static readonly IValueConverter ModifiedConverter = new FuncValueConverter<bool, string>(modified => modified ? "Modified" : "Saved");
    }
}
