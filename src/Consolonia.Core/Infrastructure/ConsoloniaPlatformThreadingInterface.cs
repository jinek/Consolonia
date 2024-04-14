using System;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     Implements special <see cref="StartTimer" />
    /// </summary>
    internal class ConsoloniaPlatformThreadingInterface : /*todo: this class does not exist anymore: InternalPlatformThreadingInterface,
    neither I can remember the purpose of this class. Seems it was used to blink the cursor,*/ IPlatformThreadingInterface
    {
        public new IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action tick)
        {
            throw new NotImplementedException();
            /*return tick.Target is DispatcherTimer { Tag: ICaptureTimerStartStop captureTimerStartStop }
                ? new ConsoloniaTextPresenterPointerBlinkFakeTimer(captureTimerStartStop)
                : base.StartTimer(priority, interval, tick);*/
        }

        public void Signal(DispatcherPriority priority)
        {
            throw new NotImplementedException();
        }

        public bool CurrentThreadIsLoopThread { get; }
        public event Action<DispatcherPriority?> Signaled;

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