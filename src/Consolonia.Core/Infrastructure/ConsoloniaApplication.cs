using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Consolonia.Core.Helpers;
using Consolonia.Core.Infrastructure;

namespace Consolonia
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

    public class ConsoloniaApplication<TMainWindow> : ConsoloniaApplication
        where TMainWindow : Window, new()
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = Activator.CreateInstance<TMainWindow>();
                ArgumentNullException.ThrowIfNull(window, typeof(TMainWindow).Name);
                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}