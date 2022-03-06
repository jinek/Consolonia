namespace Consolonia.Core.Drawing.PixelBuffer
{
    public interface ISymbol
    {
        char GetCharacter();
        bool IsWhiteSpace();
        ISymbol Blend(ref ISymbol symbolAbove);
    }
}