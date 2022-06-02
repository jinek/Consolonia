using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
using Consolonia.Core.Infrastructure;

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

        // ReSharper disable once UnusedParameter.Global
        // ReSharper disable once UnusedMember.Global
        public static TAppBuilder UseAutoDetectTerminal<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            throw new NotImplementedException();
        }

        public static TAppBuilder UseStandardConsole<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            return builder.UseConsole(new DefaultNetConsole());
        }

        public static TAppBuilder UseConsole<TAppBuilder>(this TAppBuilder builder, IConsole console)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
        {
            return builder.With(console);
        }

        public static TAppBuilder UseConsolonia<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : AppBuilderBase<TAppBuilder>, new()
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
                .LogToTrace();

            return CreateLifetime(consoloniaAppBuilder, args);
        }

        private static ClassicDesktopStyleApplicationLifetime CreateLifetime<T>(T builder, string[] args)
            where T : AppBuilderBase<T>, new()
        {
            var lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                Args = args,
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            builder.SetupWithLifetime(lifetime);
            return lifetime;
        }

        public static int StartWithConsoleLifetime<T>(
            this T builder, string[] args)
            where T : AppBuilderBase<T>, new()
        {
            ClassicDesktopStyleApplicationLifetime lifetime = CreateLifetime(builder, args);
            return lifetime.Start(args);
        }
    }
}