using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Platform;

namespace Consolonia.Core.Drawing
{
    internal sealed class ConsoloniaPlatformRenderInterfaceContext : IPlatformRenderInterfaceContext
    {
        public object TryGetFeature(Type featureType)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public IRenderTarget CreateRenderTarget(IEnumerable<object> surfaces)
        {
            return new RenderTarget(surfaces);
        }

        public IDrawingContextLayerImpl CreateOffscreenRenderTarget(PixelSize pixelSize, double scaling)
        {
            throw new NotImplementedException();
        }

        public bool IsLost => false;

        public IReadOnlyDictionary<Type, object> PublicFeatures { get; } = new Dictionary<Type, object>();
    }
}