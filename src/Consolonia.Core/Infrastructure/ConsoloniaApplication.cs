using Avalonia;
using Avalonia.Input;

namespace Consolonia.Core.Infrastructure;

public class ConsoloniaApplication : Application
{
    public override void RegisterServices()
    {
        var keyboardNavigationHandler = AvaloniaLocator.Current.GetService<IKeyboardNavigationHandler>();
        base.RegisterServices();
        AvaloniaLocator.CurrentMutable.Bind<IKeyboardNavigationHandler>().ToConstant(keyboardNavigationHandler);
    }
}