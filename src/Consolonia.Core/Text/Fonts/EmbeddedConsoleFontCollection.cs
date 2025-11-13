using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Fonts;
using Avalonia.Platform;

namespace Consolonia.Core.Text.Fonts
{
#pragma warning disable CA1310 // Specify StringComparison for correctness
    public class EmbeddedConsoleFontCollection : IFontCollection
    {
        private Dictionary<string, IGlyphTypeface> _typefaceByName = new();
        private Dictionary<Uri, IGlyphTypeface> _typefaceByUri = new();
        private List<FontFamily> _fontFamilies = new();
        private IFontManagerImpl _fontManager;
        private Uri _key;
        private Dictionary<string, Uri> _fontUris;
        private Dictionary<string, Uri[]> _fontFamilyUris;
        private bool _disposedValue;

        /// <summary>
        /// Construct a font collection from a font URI
        /// </summary>
        /// <param name="fontUri">Example: fonts:Consolonia </param>
        /// <param name="resourceUri">The a embedded resources Uri. Example: avares://MyAssembly/Fonts</param>
        public EmbeddedConsoleFontCollection(Uri fontUri, Uri resourceUri)
        {
            _key = fontUri;
            _fontUris = AssetLoader.GetAssets(resourceUri, null)
                .Where(asset => asset.AbsolutePath.EndsWith(".tlf") || asset.AbsolutePath.EndsWith(".flf"))
                .ToDictionary(uri => Path.GetFileNameWithoutExtension(uri.AbsolutePath));

            _fontFamilyUris = _fontUris// .Where(kv => Char.IsDigit(kv.Key[^1]))
                    .GroupBy(kv => kv.Key.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'))
                    .ToDictionary(g => g.Key, g => g.Select(kv => kv.Value).ToArray());
            foreach (var familyName in _fontFamilyUris.Keys)
            {
                var key = familyName.TrimStart('#');
                _fontFamilies.Add(new FontFamily(_key, $"#{key}"));
            }
        }

        public FontFamily this[int index] => _fontFamilies[index];

        public Uri Key => _key;

        public int Count => _fontUris.Count;

        public IFontManagerImpl FontManager => _fontManager;

        public IEnumerator<FontFamily> GetEnumerator()
        {
            return _fontFamilies.GetEnumerator();
        }

        public void Initialize(IFontManagerImpl fontManager)
        {
            _fontManager = fontManager;
        }

        public bool TryGetGlyphTypeface(string familyName, FontStyle style, FontWeight weight, FontStretch stretch, [NotNullWhen(true)] out IGlyphTypeface glyphTypeface)
        {
            if (_typefaceByName.TryGetValue(familyName, out glyphTypeface))
            {
                return true;
            }
            else if (_fontFamilyUris.TryGetValue(familyName, out var uris))
            {
                // load the family and all it's members
                glyphTypeface = LoadEmbeddedFontFamily(familyName, uris.ToArray());
            }
            else if (_fontUris.TryGetValue(familyName, out var uri))
            {
                // load just the font
                glyphTypeface = LoadEmbeddedFont(uri);
            }

            return glyphTypeface != null;
        }

        public bool TryMatchCharacter(int codepoint, FontStyle fontStyle, FontWeight fontWeight, FontStretch fontStretch, string familyName, CultureInfo culture, out Typeface typeface)
        {
            if (TryGetGlyphTypeface(familyName, fontStyle, fontWeight, fontStretch, out var glyphTypeface))
            {
                if (glyphTypeface.TryGetGlyph((uint)codepoint, out var glyphIndex))
                {
                    typeface = new Typeface(familyName, fontStyle, fontWeight, fontStretch);
                    return true;
                }
            }
            typeface = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Load a typeface from an embedded resource by name
        /// </summary>
        /// <param name="familyName"></param>
        /// <param name="resources"></param>
        /// <returns></returns>
        protected IGlyphTypeface LoadEmbeddedFontFamily(string familyName, params Uri[] resources)
        {
            var fontFamily = new FontFamily(Key, $"#{familyName.TrimStart('#')}");
            var familyTypespace = new AsciiFamilyTypeface(familyName);

            foreach (var resource in resources)
            {
                if (!_typefaceByUri.TryGetValue(resource, out var typeface))
                {
                    typeface = LoadEmbeddedFont(resource);
                }
                familyTypespace.AddTypeface(typeface);
            }

            _typefaceByName[familyName] = familyTypespace;
            return familyTypespace;
        }

        /// <summary>
        /// Load an embedded font from a given asset URI
        /// </summary>
        /// <param name="uri">avares: uri to the font</param>
        /// <returns></returns>
        protected IGlyphTypeface LoadEmbeddedFont(Uri uri)
        {
            var resourceName = Path.GetFileName(uri.AbsolutePath);
            var fontName = Path.GetFileNameWithoutExtension(uri.AbsolutePath);
            using var stream = AssetLoader.Open(uri);
            IGlyphTypeface typeface = null;
            if (resourceName.EndsWith(".flf"))
            {
                typeface = FigletTypefaceLoader.Load(stream, resourceName);
            }
            else if (resourceName.EndsWith(".tlf"))
            {
                typeface = CacaTypefaceLoader.Load(stream, Path.GetFileNameWithoutExtension(resourceName));
            }
            ArgumentNullException.ThrowIfNull(typeface, $"Failed to load font variation: {resourceName}");
            _typefaceByName[fontName] = typeface;
            _typefaceByUri[uri] = typeface;
            return typeface;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EmbeddedConsoleFontCollection()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}