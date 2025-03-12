using System.Collections.ObjectModel;
using Avalonia;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    /// A pixelbuffer which is composed of multiple layers
    /// </summary>
    public class PixelBufferSurface : PixelBuffer
    {
        public PixelBufferSurface(IConsole console)
            : base(console.Size)
        {
            console.Resized += () => SetBufferSize(console.Size);
        }

        // for unit tests
        public PixelBufferSurface(PixelBufferSize size)
            : base(size)
        {
        }

        public ObservableCollection<PixelBufferLayer> Layers { get; } = new();

        public PixelBufferLayer CreateLayer(PixelBufferCoordinate position, PixelBufferSize size)
            => CreateLayer((ushort)position.X, (ushort)position.Y, (ushort)size.Width, (ushort)size.Height);

        public PixelBufferLayer CreateLayer(ushort x, ushort y, ushort width, ushort height)
        {
            var layer = new PixelBufferLayer(this, x, y, width, height);
            Layers.Add(layer);
            return layer;
        }

        /// <summary>
        /// Blend all of the layers into the pixel buffer.
        /// </summary>
        public void BlendLayers()
        {
            foreach (var layer in Layers)
            {
                layer.Blend();
            }
        }
    }
}
