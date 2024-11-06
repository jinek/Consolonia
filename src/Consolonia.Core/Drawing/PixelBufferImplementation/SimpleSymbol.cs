using System.Diagnostics;
using System.Text;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Rune}'")]
    public readonly struct SimpleSymbol : ISymbol
    {
        private readonly Rune _rune;

        public SimpleSymbol(char ch)
            : this(new Rune(ch))
        {
        }

        public SimpleSymbol(Rune rune)
        {
            _rune = rune;
        }

        public Rune Rune => _rune;

        public bool IsWhiteSpace()
            => Rune.IsWhiteSpace(_rune);

        public ISymbol Blend(ref ISymbol symbolAbove)
            => symbolAbove;
    }
}