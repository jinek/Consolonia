using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Avalonia.Rendering;
using Avalonia.Threading;
using Consolonia.Core.Dummy;
using Consolonia.Core.Text;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaPlatform : IWindowingPlatform
    {
        public IWindowImpl CreateWindow()
        {
            return new ConsoleWindow();
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            RaiseNotSupported(13);
            return null!;
        }

        public ITrayIconImpl CreateTrayIcon()
        {
            throw new NotImplementedException();
        }

        public ITopLevelImpl CreateEmbeddableTopLevel()
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            NotSupported += InternalIgnore;

            AvaloniaLocator.CurrentMutable.BindToSelf(this)
                .Bind<IWindowingPlatform>().ToConstant(this)
                /*todo: need replacement? .Bind<IPlatformThreadingInterface>().ToSingleton<ConsoloniaPlatformThreadingInterface>()*/
                .Bind<IRenderTimer>().ToConstant(new UiThreadRenderTimer(120))
                .Bind<IDispatcherImpl>().ToConstant(new ManagedDispatcherImpl(null))
                /*.Bind<IRenderTimer>().ToConstant(new SleepLoopRenderTimer(120))*/
                /*SleepLoopRenderTimer : IRenderTimer*/
                /*.Bind<IRenderLoop>().ToConstant(new RenderLoop()) todo: is internal now*/
                .Bind<PlatformHotkeyConfiguration>().ToConstant(new PlatformHotkeyConfiguration(KeyModifiers.Control))
                .Bind<IKeyboardDevice>().ToConstant(new ConsoleKeyboardDevice())
                .Bind<IFontManagerImpl>().ToConstant(new FontManagerImpl())
                .Bind<ITextShaperImpl>().ToConstant(new TextShaper())
                .Bind<IMouseDevice>().ToConstant(new MouseDevice())
                .Bind<ICursorFactory>().ToConstant(new DummyCursorFactory())
                .Bind<IPlatformIconLoader>().ToConstant(new DummyIconLoader())
                .Bind<IPlatformSettings>().ToSingleton<ConsoloniaPlatformSettings>()
                .Bind<IStorageProvider>().ToSingleton<ConsoloniaStorageProvider>()
                //.Bind<IClipboard>().ToConstant(null)
                /*.Bind<IKeyboardNavigationHandler>().ToTransient<ArrowsAndKeyboardNavigationHandler>() todo: implement this navigation*/
                //.Bind<IClipboard>().ToConstant(new X11Clipboard(this))
                //.Bind<IPlatformSettings>().ToConstant(new PlatformSettingsStub())
                //.Bind<ISystemDialogImpl>().ToConstant(new GtkSystemDialog())
                /*.Bind<IMountedVolumeInfoProvider>().ToConstant(new LinuxMountedVolumeInfoProvider())*/;

        }

        [DebuggerStepThrough]
        internal static void RaiseNotSupported(int errorCode, params object[] information)
        {
            var notSupportedRequest = new NotSupportedRequest(errorCode, information);
            NotSupported?.Invoke(notSupportedRequest);
            notSupportedRequest.CheckHandled();
        }

        public static event Action<NotSupportedRequest> NotSupported;

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

    public class ConsoloniaPlatformSettings : DefaultPlatformSettings
    {
        //todo:
    }
}