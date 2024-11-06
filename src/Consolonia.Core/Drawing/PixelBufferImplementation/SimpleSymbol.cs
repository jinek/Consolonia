using System.Diagnostics;
using System.Text;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [DebuggerDisplay("'{Rune}'")]
    public readonly struct SimpleSymbol : ISymbol
    {
        public SimpleSymbol()
        {
            Rune = new Rune('\0');
        }

        public SimpleSymbol(Rune rune)
        {
            Rune = rune;
        }

        public Rune Rune { get; } = new('\0');

        public bool IsWhiteSpace()
        {
            return Rune.IsWhiteSpace(Rune);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return symbolAbove.Rune.Value != '\0' ? symbolAbove : this;
        }
    }
}