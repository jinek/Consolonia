using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.Platform;
using Avalonia.FreeDesktop;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;
using Consolonia.Core.Dummy;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaPlatform : IWindowingPlatform
    {
        public void Initialize()
        {
            NotSupported += InternalIgnore;

            AvaloniaLocator.CurrentMutable.BindToSelf(this)
                .Bind<IWindowingPlatform>().ToConstant(this)
                .Bind<IConsole>().ToConstant(new DefaultNetConsole())
                .Bind<IPlatformThreadingInterface>().ToSingleton<ConsoloniaPlatformThreadingInterface>()
                .Bind<IRenderTimer>().ToConstant(new UiThreadRenderTimer(120))
                .Bind<IRenderLoop>().ToConstant(new RenderLoop())
                .Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control))
                .Bind<IKeyboardDevice>().ToConstant(new ConsoleKeyboardDevice())
                /*.Bind<IFontManagerImpl>().ToConstant(new FontManagerImpl()) overriden by skiia in program.cs*/
                .Bind<ICursorFactory>().ToConstant(new DummyCursorFactory())
                //.Bind<IClipboard>().ToConstant(new X11Clipboard(this))
                //.Bind<IPlatformSettings>().ToConstant(new PlatformSettingsStub())
                //.Bind<ISystemDialogImpl>().ToConstant(new GtkSystemDialog())
                .Bind<IMountedVolumeInfoProvider>().ToConstant(new LinuxMountedVolumeInfoProvider());
        }

        public IWindowImpl CreateWindow()
        {
            return new ConsoleWindow();
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            RaiseNotSupported(13);
            return null;
        }

        public ITrayIconImpl? CreateTrayIcon()
        {
            throw new NotImplementedException();
        }

        [DebuggerStepThrough]
        internal static void RaiseNotSupported(int errorCode, params object[] information)
        {
            var notSupportedRequest = new NotSupportedRequest { ErrorCode = errorCode, Information = information };
            NotSupported?.Invoke(notSupportedRequest);
            notSupportedRequest.CheckHandled();
        }

        public static event Action<NotSupportedRequest>? NotSupported;

        private static void InternalIgnore(NotSupportedRequest notSupportedRequest)
        {
            switch (notSupportedRequest.ErrorCode)
            {
                case 9 when ReferenceEquals(notSupportedRequest.Information[0], Brushes.White) &&
                            notSupportedRequest.Information[1] is null &&
                            ((RoundedRect)notSupportedRequest.Information[2]).Rect.TopLeft is { X: 0, Y: 0 }:
                    notSupportedRequest.SetHandled();
                    //this is case of OverlayPopupHost.Render
                    break;
            }
        }
    }
}