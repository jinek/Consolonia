using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;

namespace Consolonia.Core.Helpers
{
    public static class Helper
    {
        public static async Task WaitDispatcherInitialized()
        {
            //todo: check if avalonia exiting to break the loop
            while (AvaloniaLocator.Current.GetService<IDispatcherImpl>() == null) await Task.Yield();
        }
    }
}