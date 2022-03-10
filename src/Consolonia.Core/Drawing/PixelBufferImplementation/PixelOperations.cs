using System;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    internal static class PixelOperations
    {
        public static ConsoleColor Shade(this ConsoleColor color)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (color)
            {
                case ConsoleColor.Black: return color;
                case ConsoleColor.White: return ConsoleColor.Gray;
                default:
                    var name = Enum.GetName(color);
                    const string dark = "Dark";
                    return !name.Contains(dark, StringComparison.Ordinal) ? Enum.Parse<ConsoleColor>(dark + name) : ConsoleColor.Black;
            }
        }

        public static (int integerFraction, int remainder) Divide(int a, int b)
        {
            return (a / b, a % b);
        }
    }
}