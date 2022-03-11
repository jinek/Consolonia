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
    }
}