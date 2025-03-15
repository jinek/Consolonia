using Avalonia;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Gallery
{
    internal static class Extensions
    {
        internal static bool IsUnitTestConsole()
        {
            var lifetime = Application.Current.ApplicationLifetime as ConsoloniaLifetime;
            var consoleWindow = lifetime?.TopLevel as ConsoleWindow;
            return (consoleWindow.Console?.GetType().Name == "UnitTestConsole");
        }
    }
}
