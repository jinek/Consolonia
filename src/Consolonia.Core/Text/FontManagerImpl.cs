using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/typography/opentype/spec/ttch01#funits-and-the-em-square
    /// </summary>
    internal class FontManagerImpl : IFontManagerImpl
    {
        public string GetDefaultFontFamilyName()
        {
            return "ConsoleDefault(F7D6533C-AC9D-4C4A-884F-7719A9B5DC0C)";
        }

        public IEnumerable<string> GetInstalledFontFamilyNames(bool checkForUpdates = false)
        {
            throw new NotImplementedException();
        }

        public bool TryMatchCharacter(
            int codepoint,
            FontStyle fontStyle,
            FontWeight fontWeight,
            FontFamily fontFamily,
            CultureInfo culture,
            out Typeface typeface)
        {
            throw new NotImplementedException();
        }

        public IGlyphTypefaceImpl CreateGlyphTypeface(Typeface typeface)
        {
            //todo: check defaults or rause platform error

            return new TypefaceImpl();
        }
    }
}