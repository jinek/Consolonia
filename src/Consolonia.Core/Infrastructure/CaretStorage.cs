using System;

namespace Consolonia.Core.Infrastructure
{
    internal class CaretStorage : IDisposable
    {
        private readonly IConsole _console;
        private readonly (ushort x, ushort y) _initialPosition;

        public CaretStorage(IConsole console)
        {
            _console = console;
            _initialPosition = console.GetCaretPosition();
        }

        public void Dispose()
        {
            _console.SetCaretPosition(_initialPosition);
        }
    }
}