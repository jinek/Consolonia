using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Consolonia.Core.Controls;
using Consolonia.Core.Helpers;

// ReSharper disable once CheckNamespace
namespace Consolonia
{
    public class ConsoloniaApplication : Application
    {
        public override void OnFrameworkInitializationCompleted()
        {
            // override AccessText to use ConsoloniaAccessText as default ContentPresenter for unknown data types (aka string)
            DataTemplates.Add(new FuncDataTemplate<object>(
                (data, _) =>
                {
                    if (data != null)
                    {
                        var result = new ConsoloniaAccessText();
                        // ReSharper disable AccessToStaticMemberViaDerivedType
                        result.Bind(TextBlock.TextProperty,
                            result.GetBindingObservable(Control.DataContextProperty, x => x?.ToString()));
                        return result;
                    }

                    return null;
                },
                true)
            );

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