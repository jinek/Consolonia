using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
                    T[] newData = readDataFunction();
                    Enqueue(newData);
                    //todo: low should continue the loop in case of exceptions?
                }
            });
        }

        private void Enqueue(IEnumerable<T> items)
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
    }
}