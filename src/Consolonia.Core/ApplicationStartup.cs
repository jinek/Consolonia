using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Consolonia.Core.Drawing;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.Core.Text;

namespace Consolonia.Core
{
    public static class ApplicationStartup
    {
        public static void StartConsolonia<TApp>() where TApp : Application, new()
        {
            AppBuilder usePlatformDetect = AppBuilder.Configure<TApp>()
                .UseWindowingSubsystem(() => new ConsoloniaPlatform().Initialize())
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

            using var lifetime = new ClassicDesktopStyleApplicationLifetime
            {
                /*Args = args,*/
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };

            app.SetupWithLifetime(lifetime);

            lifetime.Start(Array.Empty<string>());
        }
    }
}