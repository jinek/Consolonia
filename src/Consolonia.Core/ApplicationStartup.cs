using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
using Consolonia.Core.Infrastructure;

// ReSharper disable UnusedMember.Global //todo: how to disable it for public methods?

// ReSharper disable MemberCanBePrivate.Global

namespace Consolonia.Core
{
    public static class ApplicationStartup
    {
        public static void StartConsolonia<TApp>(params string[] args) where TApp : Application, new()
        {
            StartConsolonia<TApp>(new DefaultNetConsole(), args);
        }

        public static void StartConsolonia<TApp>(IConsole console, params string[] args) where TApp : Application, new()
        {
            ClassicDesktopStyleApplicationLifetime lifetime = BuildLifetime<TApp>(console, args);

            lifetime.Start(args);
        }

        public static AppBuilder UseStandardConsole(this AppBuilder builder)
        {
            return builder.UseConsole(new DefaultNetConsole());
        }

        public static AppBuilder UseConsole(this AppBuilder builder, IConsole console)
        {
            return builder.With(console);
        }

        public static AppBuilder UseConsolonia(this AppBuilder builder)
        {
            return builder
                .UseWindowingSubsystem(() => new ConsoloniaPlatform().Initialize(), nameof(ConsoloniaPlatform))
                .UseRenderingSubsystem(() =>
                {
                    var platformRenderInterface =
                        AvaloniaLocator.CurrentMutable.GetService<IPlatformRenderInterface>();

                    var consoloniaRenderInterface = new ConsoloniaRenderInterface(platformRenderInterface);

                    AvaloniaLocator.CurrentMutable
                        .Bind<IPlatformRenderInterface>().ToConstant(consoloniaRenderInterface);
                }, nameof(ConsoloniaRenderInterface));
        }

        public static ClassicDesktopStyleApplicationLifetime BuildLifetime<TApp>(IConsole console, string[] args)
            where TApp : Application, new()
        {
            AppBuilder consoloniaAppBuilder = AppBuilder.Configure<TApp>()
                .UseConsole(console)
                .UseConsolonia()
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