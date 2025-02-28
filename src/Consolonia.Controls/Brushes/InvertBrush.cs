using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

// ReSharper disable CheckNamespace
namespace Consolonia.Controls.Brushes
{
    /// <summary>
    ///     This brush inverts foreground/background colors for each pixel of the area it paints.
    /// </summary>
    public class InvertBrush : Animatable, IImmutableBrush
    {
        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;
    }
}