using System;
using System.Runtime.CompilerServices;

namespace Consolonia.Core.InternalHelpers
{
    internal static class CommonInternalHelper
    {
        public static bool IsNearlyEqual(this double value, double compareTo)
        {
            return value.CompareTo(compareTo) == 0;
        }

        public static bool IsNearlyEqual(this float value, float compareTo)
        {
            return value.CompareTo(compareTo) == 0;
        }

        public static string GetStyledPropertyName([CallerMemberName] string propertyFullName = null)
        {
            return propertyFullName![..^8];
        }

        public static T NotNull<T>(this T? t) where T : struct
        {
            if (t == null)
                throw new InvalidOperationException("Value is not expected to be null");
            return t.Value;
        }
    }
}