using System;
using System.Globalization;
using System.IO;
using Avalonia.Media;
using Avalonia.Platform;

namespace Consolonia.Core.Text
{
    /// <summary>
    ///     https://docs.microsoft.com/en-us/typography/opentype/spec/ttch01#funits-and-the-em-square
    /// </summary>
    internal class FontManagerImpl : IFontManagerImpl
    {
        public string GetDefaultFontFamilyName()
        {
            return GetTheOnlyFontFamilyName();
        }

        string[] IFontManagerImpl.GetInstalledFontFamilyNames(bool checkForUpdates)
        {
            return new[] { GetTheOnlyFontFamilyName() };
        }

        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight,
            FontStretch fontStretch,
            CultureInfo culture, out Typeface typeface)
        {
            throw new NotImplementedException();
        }

        public bool TryCreateGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch,
            out IGlyphTypeface glyphTypeface)
        {
            //todo: check font is ours the only
            glyphTypeface = new GlyphTypeface
            {
                Weight = weight,
                Style = style
            };
            return true;
        }

        public bool TryCreateGlyphTypeface(Stream stream, out IGlyphTypeface glyphTypeface)
        {
            throw new NotImplementedException();
        }

        public static string GetTheOnlyFontFamilyName()
        {
            return "ConsoleDefault(F7D6533C-AC9D-4C4A-884F-7719A9B5DC0C)";
        }
    }
}