using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Avalonia.Media;

namespace Consolonia.Core.Drawing
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BgraColor
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BgraColor(byte b, byte g, byte r, byte a)
        {
            B = b;
            G = g;
            R = r;
            A = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BgraColor FromColor(Color color)
        {
            return new BgraColor(color.B, color.G, color.R, color.A);
        }

        public static readonly BgraColor Transparent = new(0, 0, 0, 0);
    }
}