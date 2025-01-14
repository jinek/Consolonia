using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Platform;
using Consolonia.Core.Controls;
using Consolonia.Core.Drawing;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;
using Consolonia.Core.Infrastructure;

// ReSharper disable UnusedMember.Global //todo: how to disable it for public methods?
// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia
{
    public static class ApplicationStartup
    {
        public static void StartConsolonia<TApp>(params string[] args) where TApp : Application, new()
        {
            StartConsolonia<TApp>(new DefaultNetConsole(), new EgaConsoleColorMode(), args);
        }

        public static void StartConsolonia<TApp>(IConsole console, IConsoleColorMode consoleColorMode,
            params string[] args) where TApp : Application, new()
        {
            ClassicDesktopStyleApplicationLifetime lifetime = BuildLifetime<TApp>(console, consoleColorMode, args);

            lifetime.Start(args);
        }

        public static AppBuilder UseStandardConsole(this AppBuilder builder)
        {
            return builder.UseConsole(new DefaultNetConsole()).UseConsoleColorMode(new EgaConsoleColorMode());
        }

        public static AppBuilder UseConsole(this AppBuilder builder, IConsole console)
        {
            return builder.With(console)
                .With<IConsoleOutput>(console);
        }

        public static AppBuilder UseConsoleColorMode(this AppBuilder builder, IConsoleColorMode consoleColorMode)
        {
            return builder.With(consoleColorMode);
        }

        public static AppBuilder UseConsolonia(this AppBuilder builder)
        {
            return builder
                .UseStandardRuntimePlatformSubsystem()
                .UseWindowingSubsystem(() => new ConsoloniaPlatform().Initialize(), nameof(ConsoloniaPlatform))
                .UseRenderingSubsystem(() =>
                {
                    var consoloniaRenderInterface = new ConsoloniaRenderInterface();

                    AvaloniaLocator.CurrentMutable
                        .Bind<IPlatformRenderInterface>().ToConstant(consoloniaRenderInterface);
                }, nameof(ConsoloniaRenderInterface));
        }

        public static ClassicDesktopStyleApplicationLifetime BuildLifetime<TApp>(IConsole console,
            IConsoleColorMode consoleColorMode, string[] args)
            where TApp : Application, new()
        {
            AppBuilder consoloniaAppBuilder = AppBuilder.Configure<TApp>()
                .UseConsole(console)
                .UseConsolonia()
                .UseConsoleColorMode(consoleColorMode)
                .LogToException();

            return CreateLifetime(consoloniaAppBuilder, args);
        }

        private static ClassicDesktopStyleApplicationLifetime CreateLifetime(AppBuilder builder, string[] args)
        {
            var lifetime = new ConsoloniaLifetime
            {
                Args = args,
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            builder.SetupWithLifetime(lifetime);

            // Application has been instantiated here.
            // We need to initialize it
            
            // override AccessText to use ConsoloniaAccessText as default ContentPresenter for unknown data types (aka string)
            Application.Current.DataTemplates.Add(new FuncDataTemplate<object>(
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

            return lifetime;
        }

        public static int StartWithConsoleLifetime(
            this AppBuilder builder, string[] args)
        {
            ClassicDesktopStyleApplicationLifetime lifetime = CreateLifetime(builder, args);
            return lifetime.Start(args);
        }
    }
}