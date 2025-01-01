using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

// ReSharper disable CheckNamespace
namespace Consolonia.Controls
{
    /// <summary>
    ///     This brush shades the area it paints.
    /// </summary>
    public class ShadeBrush : Animatable, IImmutableBrush
    {
        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;
    }
}