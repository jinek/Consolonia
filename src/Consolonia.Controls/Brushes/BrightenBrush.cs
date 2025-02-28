using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;

// ReSharper disable CheckNamespace
namespace Consolonia.Controls.Brushes
{
    /// <summary>
    ///     This brush brightens the area it paints.
    /// </summary>
    public class BrightenBrush : Animatable, IImmutableBrush
    {
        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;
    }
}