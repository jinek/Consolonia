using System.IO;
using Avalonia.Platform;

namespace Consolonia.Core.Dummy
{
    internal class DummyIconLoader : IPlatformIconLoader, IWindowIconImpl
    {
        public IWindowIconImpl LoadIcon(string fileName)
        {
            return this;
        }

        public IWindowIconImpl LoadIcon(Stream stream)
        {
            return this;
        }

        public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
        {
            return this;
        }

        public void Save(Stream outputStream)
        {
        }
    }
}