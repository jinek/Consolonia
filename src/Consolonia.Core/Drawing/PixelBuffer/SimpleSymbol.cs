namespace Consolonia.Core.Drawing.PixelBuffer
{
    public readonly struct SimpleSymbol : ISymbol
    {
        public SimpleSymbol(char character)
        {
            Character = character;
        }

        private readonly char Character;

        char ISymbol.GetCharacter()
        {
            return Character;
        }

        public bool IsWhiteSpace()
        {
            return Character == char.MinValue;
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return symbolAbove;
        }
    }
}