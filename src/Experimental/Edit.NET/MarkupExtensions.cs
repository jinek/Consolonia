using Avalonia.Data.Converters;

namespace EditNET
{
    public static class EditConverters
    {
        public static readonly IValueConverter ModifiedConverter =
            new FuncValueConverter<bool, string>(modified => modified ? "Modified" : "Saved");
    }
}
