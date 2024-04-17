using System;
using System.Threading;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using Avalonia.Threading;

namespace Consolonia.Core.Infrastructure
{
    /// <summary>
    ///     Implements special <see cref="StartTimer" />
    /// </summary>
    internal class ConsoloniaPlatformThreadingInterface : IPlatformThreadingInterface
    {
        private int _timersCount;
        private class InternalPlatformThreadingInterface : IDisposable
        {
            private readonly ConsoloniaPlatformThreadingInterface _consoloniaPlatformThreadingInterface;

            public InternalPlatformThreadingInterface(ConsoloniaPlatformThreadingInterface consoloniaPlatformThreadingInterface, ManagedDispatcherImpl managedDispatcherImpl, DispatcherPriority priority, TimeSpan interval, Action tick)
            {
                _consoloniaPlatformThreadingInterface = consoloniaPlatformThreadingInterface;
                if (Interlocked.Increment(ref _consoloniaPlatformThreadingInterface._timersCount) > 1)
                {
                    throw new InvalidProgramException(
                        "We are expecting only one timer to be active at a time. Implement several timers");
                }

                throw new NotImplementedException();
                /*managedDispatcherImpl.UpdateTimer(priority, interval, tick);*/
            }

            public void Dispose()
            {
                _consoloniaPlatformThreadingInterface._timersCount--;
            }
        }
        
        public IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action tick)
        {
            _managedDispatcherImpl ??= new ManagedDispatcherImpl(null); // todo: threading

            return tick.Target is DispatcherTimer { Tag: ICaptureTimerStartStop captureTimerStartStop }
                ? new ConsoloniaTextPresenterPointerBlinkFakeTimer(captureTimerStartStop)
                : new InternalPlatformThreadingInterface(this, _managedDispatcherImpl,priority, interval, tick) /*base.StartTimer(priority, interval, tick)*/;
        }

        public void Signal(DispatcherPriority priority)
        {
            throw new NotImplementedException();
        }

        public bool CurrentThreadIsLoopThread => CurrentThreadIsLoopThreadInternal;
#pragma warning disable CA1802
        // ReSharper disable once ThreadStaticFieldHasInitializer
        [ThreadStatic] private static readonly bool CurrentThreadIsLoopThreadInternal = true;
        private ManagedDispatcherImpl? _managedDispatcherImpl = null;
#pragma warning restore CA1802
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