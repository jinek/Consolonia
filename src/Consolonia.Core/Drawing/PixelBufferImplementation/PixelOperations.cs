using System;
using Avalonia.Media;

// ReSharper disable UnusedMember.Global

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    internal static class PixelOperations
    {
        public static Color Shade(this Color color)
        {
            var factor = 0.8f;
            return Color.FromRgb((byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));
            //// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            //switch (color)
            //{
            //    case Colors.Black: return color;
            //    case Colors.White: return ConsoleColor.Gray;
            //    default:
            //        string name = DimEnum.GetName(color) ?? throw new NotImplementedException();
            //        const string dark = "Dark";
            //        return !name.Contains(dark, StringComparison.Ordinal)
            //            ? Enum.Parse<ConsoleColor>(dark + name)
            //            : ConsoleColor.Black;
            //}
        }

        public static (int integerFraction, int remainder) Divide(int a, int b)
        {
            return (a / b, a % b);
        }
    }
}