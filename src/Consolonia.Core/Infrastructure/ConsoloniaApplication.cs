using Avalonia;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaApplication : Application
    {
        public override void RegisterServices()
        {
            base.RegisterServices();
            var keyboardNavigationHandler = AvaloniaLocator.Current.GetRequiredService<IKeyboardNavigationHandler>();
            AvaloniaLocator.CurrentMutable.Bind<IKeyboardNavigationHandler>()
                .ToConstant(new ArrowsAndKeyboardNavigationHandler(keyboardNavigationHandler));
        }
    }
}