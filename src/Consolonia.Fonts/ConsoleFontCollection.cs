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
    internal sealed class ConsoleFontCollection : FontCollectionBase
    {
        private Dictionary<string, IGlyphTypeface> _typefaceByName = new();
        private Dictionary<Uri, IGlyphTypeface> _typefaceByUri = new();
        private List<FontFamily> _fontFamilies = new();
        private IFontManagerImpl _fontManager;
        private Uri _key;

        public ConsoleFontCollection()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _key = new Uri("fonts:Consolonia");
            LoadTypeface("BigFig.flf");
            LoadTypeface("Mini.flf");
            LoadTypeface("Braille.tlf");
            LoadTypeface("Emboss.tlf");
            LoadTypeface("Circle.tlf");
            LoadTypeface("WideTerm.tlf");
            LoadTypeface("Benjamin.flf");
            LoadTypeface("Contessa.flf");
            LoadTypeface("Cursive.flf");
            LoadTypeface("Cygnet.flf");
            LoadTypeface("Digital.flf");
            LoadTypeface("Doom.flf");
            LoadTypeface("Graffiti.flf");
            LoadTypeface("Pepper.flf");
            LoadTypeface("Rectangles.flf");
            LoadTypeface("Short.flf");
            LoadTypeface("Slant.flf");  
            LoadTypeface("Straight.flf");
            LoadFamily("Point", "Point2.flf", "Point3.flf");
            LoadFamily("Standard", "Standard5.flf", "Standard6.flf", "Standard8.flf");
            LoadFamily("Mono", "Mono9.tlf", "Mono12.tlf");
            LoadFamily("BigMono", "BigMono9.tlf", "BigMono12.tlf");
            LoadFamily("SmallMono", "SmallMono9.tlf", "SmallMono12.tlf");
            sw.Stop();
            Debug.WriteLine("Fonts loaded in " + sw.ElapsedMilliseconds + " ms");
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
            return _typefaceByName.TryGetValue(familyName, out glyphTypeface);
        }

        private void LoadFamily(string familyName, params string[] variations)
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