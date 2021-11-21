using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Consolonia.Core.Styles
{
    // Copypaste from FluentTheme from Avalonia
    public abstract class ResourceIncludeBase : IResourceProvider, IStyle
    {
        private readonly Uri _baseUri;
        private bool _isLoading;
        private IStyle[]? _loaded;

        protected ResourceIncludeBase(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        protected ResourceIncludeBase(IServiceProvider serviceProvider)
        {
            _baseUri = ((IUriContext)serviceProvider.GetService(typeof(IUriContext))).BaseUri;
        }

        protected abstract Uri Uri { get; }

        /// <summary>
        ///     Gets the loaded style.
        /// </summary>
        public IStyle Loaded
        {
            get
            {
                if (_loaded == null)
                {
                    _isLoading = true;
                    var loaded = (IStyle)AvaloniaXamlLoader.Load(Uri, _baseUri);
                    _loaded = new[] { loaded };
                    _isLoading = false;
                }

                return _loaded?[0]!;
            }
        }

        public bool TryGetResource(object key, out object? value)
        {
            if (!_isLoading && Loaded is IResourceProvider p) return p.TryGetResource(key, out value);

            value = null;
            return false;
        }

        bool IResourceNode.HasResources => (Loaded as IResourceProvider)?.HasResources ?? false;

        void IResourceProvider.AddOwner(IResourceHost owner)
        {
            (Loaded as IResourceProvider)?.AddOwner(owner);
        }

        void IResourceProvider.RemoveOwner(IResourceHost owner)
        {
            (Loaded as IResourceProvider)?.RemoveOwner(owner);
        }

        IResourceHost? IResourceProvider.Owner => (Loaded as IResourceProvider)?.Owner;

        public event EventHandler OwnerChanged
        {
            add
            {
                if (Loaded is IResourceProvider rp) rp.OwnerChanged += value;
            }
            remove
            {
                if (Loaded is IResourceProvider rp) rp.OwnerChanged -= value;
            }
        }

        public SelectorMatchResult TryAttach(IStyleable target, IStyleHost? host)
        {
            return Loaded.TryAttach(target, host);
        }

        IReadOnlyList<IStyle> IStyle.Children => _loaded ?? Array.Empty<IStyle>();
    }
}