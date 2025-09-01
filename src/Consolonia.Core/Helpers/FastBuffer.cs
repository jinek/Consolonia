using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Logging;

namespace Consolonia.Core.Helpers
{
    public sealed class FastBuffer<T>(Func<T[]> readDataFunction) : IDisposable
    {
        private readonly object _lock = new();
        private readonly ManualResetEvent _manualResetEvent = new(false);
        private readonly Queue<T> _queue = new(1);
        private readonly object _readingLock = new();
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            //todo: low return when loop exited
            //todo: low will not unblock reading or writing threads.
        }

        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                while (!_disposed)
                {
                    try
                    {
                        T[] newData = readDataFunction();
                        if (!newData.Any())
                            throw new InvalidOperationException("No data read from the source.");

                        Enqueue(newData);
                    }
                    catch (Exception exception)
                    {
                        if (!LogOrThrow(exception))
                            throw;
                    }
                }
            });
        }

        private void Enqueue(IReadOnlyCollection<T> items)
        {
            lock (_lock)
            {
                foreach (T item in items)
                    _queue.Enqueue(item);

                _manualResetEvent.Set();
            }
        }

        public T[] Dequeue()
        {
            lock (_readingLock)
            {
                bool waitOne = false;

                lock (_lock)
                {
                    if (_queue.Count == 0) waitOne = true;
                }

                if (waitOne)
                    _manualResetEvent.WaitOne();

                lock (_lock)
                {
                    T[] result = [.. _queue];
                    _queue.Clear();
                    _manualResetEvent.Reset();
                    return result;
                }
            }
        }

#pragma warning disable CA1000 //todo:
        public static bool LogOrThrow(Exception exception)
#pragma warning restore CA1000
        {
            ParametrizedLogger? logger = Logger.TryGet(LogEventLevel.Error, "Consolonia");
            if (logger != null)
            {
                ((ParametrizedLogger)logger).Log("Consolonia","Exception in Curses event loop: {Exception}", exception);
                return true;
            }

            return false;
        }
    }
}