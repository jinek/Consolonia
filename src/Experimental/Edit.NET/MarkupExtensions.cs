using System.IO;
using Avalonia.Data.Converters;

namespace EditNET
{
    public static class EditConverters
    {
        public static readonly IValueConverter ModifiedConverter =
            new FuncValueConverter<bool, string>(modified => modified ? "Modified" : "Saved");

        public static readonly IValueConverter FilePathToNameConverter =
            new FuncValueConverter<string, string>(filePath => Path.GetFileName(filePath)!);
    }
}