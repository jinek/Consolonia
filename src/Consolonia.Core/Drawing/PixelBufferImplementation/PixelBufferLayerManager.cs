using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;

namespace Consolonia.Core.Drawing.PixelBufferImplementation
{
    internal class PixelBufferLayerManager
    {
        public PixelBufferLayerManager(PixelSize size)
            : this((ushort)size.Width, (ushort)size.Height)
        { }

        public PixelBufferLayerManager(ushort width, ushort height)
        {
            PixelBuffer = new PixelBuffer((ushort)width, (ushort)height); 
        }

        public PixelBuffer PixelBuffer { get; set; }

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
        public void Blend()
        {
            foreach (var layer in Layers)
            {
                layer.Blend(PixelBuffer);
            }
        }
    }
}
