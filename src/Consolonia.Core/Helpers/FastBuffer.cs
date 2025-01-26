using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Consolonia.Core.Helpers
{
    public sealed class FastBuffer<T>(Func<T> readDataFunction) : IDisposable
    {
        private readonly ManualResetEvent _manualResetEvent = new(false);

        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                while (!_disposed)
                {
                    T newData = readDataFunction();
                    Enqueue(newData);
                    //todo: low should continue the loop in case of exceptions?
                }
            });
        }

        private readonly object _lock = new();
        private readonly object _readingLock = new();
        private readonly Queue<T> _queue = new(1);
        private bool _disposed;

        private void Enqueue(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
                _manualResetEvent.Set();
            }
        }

        private void Enqueue(T[] items)
        {
            lock (_lock)
            {
                foreach (T item in items)
                {
                    Enqueue(item);
                }
            }
        }

        public T[] Dequeue()
        {
            lock (_readingLock)
            {
                bool waitOne = false;

                lock (_lock)
                {
                    if (_queue.Count == 0)
                    {
                        waitOne = true;
                    }
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

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;

            //todo: low return when loop exited
            //todo: low will not unblock reading or writing threads.
        }
    }
}