namespace Consolonia.Core.Infrastructure
{
    public struct ConsolePosition
    {
        public ConsolePosition(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public ushort X { get; }
        public ushort Y { get; }

        public void Deconstruct(out ushort x, out ushort y)
        {
            x = X;
            y = Y;
        }

        public static explicit operator ConsolePosition((ushort x, ushort y) val) => new(val.x, val.y);

        public ConsolePosition WithXpp()
        {
            return new ConsolePosition((ushort)(X + 1), Y);
        }
    }
}