using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.Text;

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

        public static ClassicDesktopStyleApplicationLifetime BuildLifetime<TApp>(IConsole console, string[] args)
            where TApp : Application, new()
        {
            AppBuilder usePlatformDetect = AppBuilder.Configure<TApp>()
                .UseWindowingSubsystem(() => new ConsoloniaPlatform().Initialize(console))
                .UseRenderingSubsystem(() =>
                    {
                        var platformRenderInterface =
                            AvaloniaLocator.CurrentMutable.GetService<IPlatformRenderInterface>();

                        var myRenderInterface = new RenderInterface(platformRenderInterface);

                        AvaloniaLocator.CurrentMutable
                            .Bind<IPlatformRenderInterface>().ToConstant(myRenderInterface)
                            .Bind<IFontManagerImpl>().ToConstant(new FontManagerImpl())
                            .Bind<ITextShaperImpl>().ToConstant(new TextShaperImpl());
                        AvaloniaLocator.CurrentMutable.Bind<IPlatformIconLoader>().ToConstant(new DummyIconLoader());
                    },
                    "Consolonia");

            AppBuilder app = usePlatformDetect
                .LogToTrace();

            var lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                Args = args,
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };

            app.SetupWithLifetime(lifetime);
            return lifetime;
        }
    }
}