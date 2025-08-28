using System;
using System.Collections.Generic;
using System.Linq;

namespace Consolonia.PlatformSupport
{
    internal static class Helpers
    {
        /// <summary>
        ///     From https://stackoverflow.com/a/66275102/2362847
        /// </summary>
        public static IEnumerable<T> GetFlags<T>(this T en) where T : struct, Enum
        {
#pragma warning disable CA2248
            return Enum.GetValues<T>().Where(member => en.HasFlag(member));
#pragma warning restore CA2248
        }
    }
}