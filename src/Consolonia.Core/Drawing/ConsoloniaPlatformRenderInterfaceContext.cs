using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Platform;
using Consolonia.Core.Infrastructure;

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
            var windowImpl = surfaces.Cast<ConsoleWindowImpl>().Single();
            return new RenderTarget(windowImpl);
        }

        public IDrawingContextLayerImpl CreateOffscreenRenderTarget(PixelSize pixelSize, double scaling)
        {
            throw new NotImplementedException();
        }

        public bool IsLost => false;

        public IReadOnlyDictionary<Type, object> PublicFeatures { get; } = new Dictionary<Type, object>();
    }
}