namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public interface ISymbol
    {
        char GetCharacter();
        bool IsWhiteSpace();
        ISymbol Blend(ref ISymbol symbolAbove);
    }
}