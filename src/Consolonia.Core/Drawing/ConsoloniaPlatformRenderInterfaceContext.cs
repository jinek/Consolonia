using System;
using System.Collections.Generic;
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

        public bool IsLost => false;
    }
}