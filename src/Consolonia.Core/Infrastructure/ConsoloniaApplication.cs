using Avalonia;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaApplication : Application
    {
        public override void RegisterServices()
        {
            base.RegisterServices();

            AvaloniaLocator.CurrentMutable.Bind<IKeyboardNavigationHandler>()
                .ToTransient<ArrowsAndKeyboardNavigationHandler>();
        }
    }
}