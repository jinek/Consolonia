using System.Collections.ObjectModel;
using Avalonia;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    /// <summary>
    /// A pixelbuffer which is composed of multiple layers
    /// </summary>
    internal class PixelBufferSurface : PixelBuffer
    {
        public PixelBufferSurface(ushort width, ushort height)
            : base(width, height)
        {
        }

        public PixelBufferSurface(PixelSize size)
            : this((ushort)size.Width, (ushort)size.Height)
        { }

        public PixelBufferSurface(IConsole console)
            : this(console.Size.Width, console.Size.Height)
        {
            console.Resized += () => SetBufferSize(console.Size.Width, console.Size.Height);
        }

        public ObservableCollection<PixelBufferLayer> Layers { get; } = new();

        public PixelBufferLayer CreateLayer(PixelPoint position, PixelSize size)
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
