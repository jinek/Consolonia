using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consolonia.Core.Helpers
{
    public class CustomBuffer<T> : IDisposable
    {
        private readonly Func<T> _readDataFunction;

        public CustomBuffer(Func<T> readDataFunction)
        {
            _readDataFunction = readDataFunction;
        }

        public Task RunAsync()
        {
            return Task.Run(() =>
            {
                while (!_disposed)
                {
                    T newData = _readDataFunction();
                    Enqueue(newData);
                }
            });
        }

        private readonly object _lock = new();
        private readonly Queue<T> _queue = new(1);
        private bool _disposed;

        protected void Enqueue(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
            }
        }

        protected void Enqueue(T[] items)
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
            lock (_lock)
            {
                T[] result = [.. _queue];
                _queue.Clear();
                return result;
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            
            //todo: low return when loop exited
        }
    }
}