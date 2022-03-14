using System.Globalization;
using System.Reflection;
using Avalonia.Threading;

namespace Consolonia.TestsCore
{
    internal static class InternalHelpers
    {
        public static void UpdateServicesExtension(this Dispatcher dispatcher)
        {
            typeof(Dispatcher).InvokeMember("UpdateServices",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null,
                dispatcher, null, CultureInfo.InvariantCulture);
        }
    }
}