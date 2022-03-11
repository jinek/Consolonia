using System;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     Implements special <see cref="StartTimer" />
    /// </summary>
    internal class ConsoloniaPlatformThreadingInterface : InternalPlatformThreadingInterface,
        IPlatformThreadingInterface
    {
        public new IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action tick)
        {
            return tick.Target is DispatcherTimer { Tag: ICaptureTimerStartStop captureTimerStartStop }
                ? new ConsoloniaTextPresenterPointerBlinkFakeTimer(captureTimerStartStop)
                : base.StartTimer(priority, interval, tick);
        }

        private class ConsoloniaTextPresenterPointerBlinkFakeTimer : IDisposable
        {
            private readonly ICaptureTimerStartStop _captureTimerStartStop;

            public ConsoloniaTextPresenterPointerBlinkFakeTimer(ICaptureTimerStartStop captureTimerStartStop)
            {
                _captureTimerStartStop = captureTimerStartStop;
                _captureTimerStartStop.CaptureTimerStart();
            }


            public void Dispose()
            {
                _captureTimerStartStop.CaptureTimerStop();
            }
        }
    }
}