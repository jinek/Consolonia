using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using Avalonia.Media;
using Avalonia.Platform;
using Consolonia.Core.Text.Fonts;

namespace Consolonia.Core.Text
{
    /// <summary>
    ///     https://docs.microsoft.com/en-us/typography/opentype/spec/ttch01#funits-and-the-em-square
    /// </summary>
    internal class FontManagerImpl : IFontManagerImpl
    {
        public string GetDefaultFontFamilyName()
        {
            return ConsoleDefaultFontFamily();
        }

        string[] IFontManagerImpl.GetInstalledFontFamilyNames(bool checkForUpdates)
        {
            return new[] { ConsoleDefaultFontFamily() };
        }

        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight,
            FontStretch fontStretch,
            CultureInfo culture, out Typeface typeface)
        {
            typeface = new Typeface(ConsoleDefaultFontFamily(), fontStyle, fontWeight, fontStretch);
            return true;
        }

        public bool TryCreateGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch,
            out IGlyphTypeface glyphTypeface)
        {
            if (familyName == ConsoleDefaultFontFamily())
            {
                //todo: check font is ours the only
                glyphTypeface = new ConsoleTypeface
                {
                    Weight = weight,
                    Style = style
                };
                return true;
            }

            glyphTypeface = null;
            return false;
        }

#pragma warning disable CA1822 // Mark members as static
        public bool TryCreateGlyphTypeface(Stream stream, FontSimulations fontSimulations,
            [NotNullWhen(true)] out IGlyphTypeface glyphTypeface)
#pragma warning restore CA1822 // Mark members as static
        {
            glyphTypeface = new ConsoleTypeface();
            return true;
        }

#pragma warning disable CA1822 // Mark members as static
        public bool TryCreateGlyphTypeface(Stream stream, out IGlyphTypeface glyphTypeface)
#pragma warning restore CA1822 // Mark members as static
        {
            glyphTypeface = new ConsoleTypeface();
            return true;
        }

        public static string ConsoleDefaultFontFamily()
        {
            return "ConsoleDefault";
        }
    }
}