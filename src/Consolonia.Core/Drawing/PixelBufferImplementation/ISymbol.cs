using Newtonsoft.Json;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    [JsonConverter(typeof(SymbolConverter))]
    public interface ISymbol
    {
        /// <summary>
        ///     The text for the symbol (This can be single character or unicode encoding for Emoji's and the like)
        /// </summary>
        string Text { get; }

        /// <summary>
        ///     The number of characters the symbol takes up
        /// </summary>
        ushort Width { get; }

        bool IsWhiteSpace();

        /// <summary>
        ///     Blend 2 symbols together
        /// </summary>
        /// <param name="symbolAbove"></param>
        /// <returns></returns>
        ISymbol Blend(ref ISymbol symbolAbove);
    }
}