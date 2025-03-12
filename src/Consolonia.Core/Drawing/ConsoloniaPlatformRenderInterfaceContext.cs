using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Platform;
using Consolonia.Core.Drawing.PixelBufferImplementation;

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
            var surface = surfaces.Cast<PixelBufferSurface>().Single();
            return new RenderTarget(surface, surface.Layers.First());
        }

        public IDrawingContextLayerImpl CreateOffscreenRenderTarget(PixelSize pixelSize, double scaling)
        {
            throw new NotImplementedException();
        }

        public bool IsLost => false;

        public IReadOnlyDictionary<Type, object> PublicFeatures { get; } = new Dictionary<Type, object>();
    }
}