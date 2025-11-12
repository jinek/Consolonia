using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Platform;
using Consolonia.Core.Text.Fonts;

namespace Consolonia.Fonts
{
#pragma warning disable CA1310 // Specify StringComparison for correctness
    internal sealed class ConsoleFontCollection : FontCollectionBase
    {
        private Dictionary<string, IGlyphTypeface> _typefaceByName = new();
        private Dictionary<Uri, IGlyphTypeface> _typefaceByUri = new();
        private List<FontFamily> _fontFamilies = new();
        private IFontManagerImpl _fontManager;
        private Uri _key;

        public ConsoleFontCollection()
        {
            _key = new Uri("fonts:Consolonia");
        }

        public override FontFamily this[int index] => _fontFamilies[index];

        public override Uri Key => _key;

        public override int Count => _glyphTypefaceCache.Count;

        public override IEnumerator<FontFamily> GetEnumerator()
        {
            return _fontFamilies.GetEnumerator();
        }

        public override void Initialize(IFontManagerImpl fontManager)
        {
            _fontManager = fontManager;
        }

        public override bool TryGetGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface glyphTypeface)
        {
            if (_typefaceByName.TryGetValue(familyName, out glyphTypeface))
            {
                return true;
            }
            // try to load on demand
            glyphTypeface = familyName switch
            {
                "BigFig" => LoadTypeface("BigFig.flf"),
                "Mini" => LoadTypeface("Mini.flf"),
                "Braille" => LoadTypeface("Braille.tlf"),
                "Emboss" => LoadTypeface("Emboss.tlf"),
                "Circle" => LoadTypeface("Circle.tlf"),
                "WideTerm" => LoadTypeface("WideTerm.tlf"),
                "Benjamin" => LoadTypeface("Benjamin.flf"),
                "Contessa" => LoadTypeface("Contessa.flf"),
                "Cursive" => LoadTypeface("Cursive.flf"),
                "Cygnet" => LoadTypeface("Cygnet.flf"),
                "Digital" => LoadTypeface("Digital.flf"),
                "Doom" => LoadTypeface("Doom.flf"),
                "Graffiti" => LoadTypeface("Graffiti.flf"),
                "Pepper" => LoadTypeface("Pepper.flf"),
                "Rectangles" => LoadTypeface("Rectangles.flf"),
                "Short" => LoadTypeface("Short.flf"),
                "Slant" => LoadTypeface("Slant.flf"),
                "Straight" => LoadTypeface("Straight.flf"),
                "Point" => LoadFamily("Point", "Point2.flf", "Point3.flf"),
                "Standard" => LoadFamily("Standard", "Standard5.flf", "Standard6.flf", "Standard8.flf"),
                "Mono" => LoadFamily("Mono", "Mono9.tlf", "Mono12.tlf"),
                "BigMono" => LoadFamily("BigMono", "BigMono9.tlf", "BigMono12.tlf"),
                "SmallMono" => LoadFamily("SmallMono", "SmallMono9.tlf", "SmallMono12.tlf"),
                _ => null
            };
            return glyphTypeface != null;
        }

        private IGlyphTypeface LoadFamily(string familyName, params string[] variations)
        {
            var fontFamily = new FontFamily(Key, $"#{familyName.TrimStart('#')}");
            var familyTypespace = new AsciiFamilyTypeface(familyName);

            foreach (var variation in variations)
            {
                var typeface = LoadTypeface(variation);
                familyTypespace.AddTypeface(typeface);
            }

            _typefaceByName[familyName] = familyTypespace;
            _fontFamilies.Add(fontFamily);
            return familyTypespace;
        }

        private IGlyphTypeface LoadTypeface(string variation)
        {
            var uri = new Uri($"avares://Consolonia.Fonts/Fonts/{variation}");
            var name = Path.GetFileNameWithoutExtension(variation);
            using var stream = AssetLoader.Open(uri);
            IGlyphTypeface typeface = null;
            if (variation.EndsWith(".flf"))
            {
                typeface = FigletTypefaceLoader.Load(stream, name);
            }
            else if (variation.EndsWith(".tlf"))
            {
                typeface = CacaTypefaceLoader.Load(stream, Path.GetFileNameWithoutExtension(variation));
            }
            ArgumentNullException.ThrowIfNull(typeface, $"Failed to load font variation: {variation}");
            _typefaceByName[name] = typeface;
            _typefaceByUri[uri] = typeface;
            return typeface;
        }
    }
}