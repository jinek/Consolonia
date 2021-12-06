namespace Consolonia.Core.Infrastructure
{
    public struct ConsoleSize
    {
        public ConsoleSize(ushort width, ushort height)
        {
            RightBottom = new ConsolePosition(width, height);
        }

        public ConsolePosition RightBottom { get; }
        public ushort Width => RightBottom.X;
        public ushort Height => RightBottom.Y;
    }
}