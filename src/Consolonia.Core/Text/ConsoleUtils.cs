using Avalonia.Media;

namespace Consolonia.Core.Text
{
    /// <summary>
    /// ANSI escape definitions and utility methods.
    /// </summary>
    internal static class ConsoleUtils
    {
        public const string Normal = $"\u001b[22m";
        
        public const string Bold = $"\u001b[1m";
        
        public const string Dim = $"\u001b[2m";
        
        public const string Italic = $"\u001b[3m";
        
        public const string Underline = $"\u001b[4m";
        
        public const string Strikethrough = $"\u001b[9m";

        public const string Reset = "\u001b[0m";

        public static string Foreground(Color color)
            => Foreground(color.R, color.G, color.B);

        public static string Foreground(byte red, byte green, byte blue)
            => $"\u001b[38;2;{red};{green};{blue}m";

        public static string Background(Color color)
            => Background(color.R, color.G, color.B);

        public static string Background(byte red, byte green, byte blue)
            => $"\u001b[48;2;{red};{green};{blue}m";
    }
}
