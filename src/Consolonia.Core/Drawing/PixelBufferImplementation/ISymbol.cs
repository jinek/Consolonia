using System.Text;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    public interface ISymbol
    {
        /// <summary>
        /// The rune for the symbol
        /// </summary>
        Rune Rune { get; }

        bool IsWhiteSpace();

        /// <summary>
        /// Blend 2 symbols together
        /// </summary>
        /// <param name="symbolAbove"></param>
        /// <returns></returns>
        ISymbol Blend(ref ISymbol symbolAbove);
    }
}