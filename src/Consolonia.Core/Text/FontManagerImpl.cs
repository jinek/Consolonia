using System;
using System.Diagnostics.CodeAnalysis;
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

#pragma warning disable CA1822 // Mark members as static
        public bool TryCreateGlyphTypeface(Stream stream, FontSimulations fontSimulations,
            [NotNullWhen(true)] out IGlyphTypeface glyphTypeface)
#pragma warning restore CA1822 // Mark members as static
        {
            glyphTypeface = new GlyphTypeface();
            return true;
        }

#pragma warning disable CA1822 // Mark members as static
        public bool TryCreateGlyphTypeface(Stream stream, out IGlyphTypeface glyphTypeface)
#pragma warning restore CA1822 // Mark members as static
        {
            glyphTypeface = new GlyphTypeface();
            return true;
        }

        public static string GetTheOnlyFontFamilyName()
        {
            return "ConsoleDefault(F7D6533C-AC9D-4C4A-884F-7719A9B5DC0C)";
        }
    }
}