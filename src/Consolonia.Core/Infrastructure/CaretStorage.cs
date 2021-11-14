using System;

namespace Consolonia.Core.Infrastructure
{
    internal class CaretStorage : IDisposable
    {
        private readonly IConsole _console;
        private readonly (ushort x, ushort y) _initialPosition;
        private readonly bool _caretVisible;

        public CaretStorage(IConsole console)
        {
            _console = console;
            _initialPosition = console.GetCaretPosition();
            _caretVisible = console.CaretVisible;
        }

        public void Dispose()
        {
            _console.SetCaretPosition(_initialPosition);
            _console.CaretVisible = _caretVisible;
        }
    }
}