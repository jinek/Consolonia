using System.Diagnostics;
using System.Text;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Rune}'")]
    public readonly struct SimpleSymbol : ISymbol
    {
        private readonly Rune _rune = new Rune('\0');

        public SimpleSymbol()
        {
            _rune = new Rune('\0');
        }

        public SimpleSymbol(Rune rune)
        {
            _rune = rune;
        }

        public Rune Rune => _rune;

        public bool IsWhiteSpace()
            => Rune.IsWhiteSpace(_rune);

        public ISymbol Blend(ref ISymbol symbolAbove)
            => (symbolAbove.Rune.Value != '\0') ? symbolAbove : this;
    }
}