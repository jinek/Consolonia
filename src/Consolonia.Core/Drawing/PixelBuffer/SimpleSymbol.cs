namespace Consolonia.Core.Drawing.PixelBuffer
{
    public readonly struct SimpleSymbol : ISymbol
    {
        public SimpleSymbol(char character)
        {
            Character = character;
        }

        public readonly char Character;

        char ISymbol.GetCharacter()
        {
            return Character == char.MinValue ? ' ' : Character;
        }

        public bool IsWhiteSpace()
        {
            return char.IsWhiteSpace(Character);
        }

        public ISymbol Blend(ref ISymbol symbolAbove)
        {
            return symbolAbove;
        }
    }
}