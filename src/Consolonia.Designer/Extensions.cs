using Avalonia.Controls;
using Avalonia;
using Consolonia.Core;

namespace Consolonia.Designer
{
    public static class Extensions
    {
        public static AppBuilder UseConsoloniaDesigner(this AppBuilder builder)
        {
#if DEBUG
            if (Design.IsDesignMode) //AppDomain.CurrentDomain.FriendlyName == "Avalonia.Designer.HostApp")
            {
                return builder
                    .UsePlatformDetect()
                    .WithInterFont()
                    .LogToTrace();
            }
#endif

            return builder.UseConsolonia();
        }
    }
}
