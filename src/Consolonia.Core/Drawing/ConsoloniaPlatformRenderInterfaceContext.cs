using System;
using System.Collections.Generic;
using Avalonia.Platform;
using Consolonia.Core.Drawing;

public class ConsoloniaPlatformRenderInterfaceContext : IPlatformRenderInterfaceContext
{
    public object TryGetFeature(Type featureType)
    {
        throw new NotImplementedException();
        /*return _graphicsApiContext.TryGetFeature(featureType);*/
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