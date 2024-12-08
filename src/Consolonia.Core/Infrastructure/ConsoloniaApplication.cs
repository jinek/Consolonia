using System;
using Avalonia;
using Avalonia.Input;
using Consolonia.Core.Helpers;

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

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            this.AddConsoloniaDesignMode();
        }
    }
}