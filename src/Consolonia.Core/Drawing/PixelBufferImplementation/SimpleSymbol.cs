namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public readonly struct SimpleSymbol : ISymbol
    {
        public SimpleSymbol(char character)
        {
            _character = character;
        }

        private readonly char _character;

        char ISymbol.GetCharacter()
        {
            return _character;
        }

        public bool IsWhiteSpace()
        {
            return _character == char.MinValue;
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            if (symbolAbove.IsWhiteSpace())
                return this;
            return symbolAbove;
        }
    }
}