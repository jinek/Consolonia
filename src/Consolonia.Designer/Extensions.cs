using Avalonia;
using Avalonia.Controls;
using Consolonia.Core;

namespace Consolonia.Designer
{
    public static class Extensions
    {
        /// <summary>
        ///     This method is used to initialize consolonia
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <remarks>
        ///     In Design mode it will load up avalonia instead of consolonia so that avalonia
        ///     previewer will attempt to render it. This mostly only works for text layout.
        ///     In release mode it will load consolonia, removing the dependency on the desktop avalonia subsystem.
        ///     NOTE: The package references for
        /// </remarks>
        public static AppBuilder UseConsoloniaDesigner(this AppBuilder builder)
        {
#if DEBUG
            if (Design.IsDesignMode) //AppDomain.CurrentDomain.FriendlyName == "Avalonia.Designer.HostApp")
                return builder
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace();
#endif

            return builder.UseConsolonia();
        }
    }
}