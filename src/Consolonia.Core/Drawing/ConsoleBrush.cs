using System;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    public class ConsoleBrush : AvaloniaObject, IImmutableBrush
    {
        public static readonly StyledProperty<Color> ColorProperty =
            AvaloniaProperty.Register<ConsoleBrush, Color>(nameof(Color));

        public static readonly StyledProperty<PixelBackgroundMode> ModeProperty =
            AvaloniaProperty.Register<ConsoleBrush, PixelBackgroundMode>(nameof(Mode));

        // ReSharper disable once UnusedMember.Global
        public ConsoleBrush(Color color, PixelBackgroundMode mode) : this(color)
        {
            Mode = mode;
        }

        public ConsoleBrush(Color color)
        {
            Color = color;
        }

        public ConsoleBrush()
        {
        }

        public PixelBackgroundMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public Color Color
        {
            get => GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;

        /// <summary>
        ///     Convert a IBrush to a Brush.
        /// </summary>
        /// <remarks>
        ///     NOTE: If it's a ConsoleBrush it will be passed through unchanged, unless mode is set then it will convert
        ///     consolebrush to mode
        /// </remarks>
        /// <param name="brush"></param>
        /// <param name="mode">Default is Colored.</param>
        /// <returns></returns>
        public static ConsoleBrush FromBrush(IBrush brush, PixelBackgroundMode? mode = null)
        {
            ArgumentNullException.ThrowIfNull(brush, nameof(brush));

            switch (brush)
            {
                case ConsoleBrush consoleBrush:
                    if (mode != null && consoleBrush.Mode != mode)
                        return new ConsoleBrush(consoleBrush.Color, mode.Value);
                    return consoleBrush;

                case LineBrush lineBrush:
                    switch (lineBrush.Brush)
                    {
                        case ConsoleBrush consoleBrush:
                            return consoleBrush;
                        case ISolidColorBrush br:
                            return new ConsoleBrush(br.Color, mode ?? PixelBackgroundMode.Colored);
                        default:
                            ConsoloniaPlatform.RaiseNotSupported(6);
                            return null;
                    }

                case ISolidColorBrush solidBrush:
                    return new ConsoleBrush(solidBrush.Color, mode ?? PixelBackgroundMode.Colored);

                default:
                    ConsoloniaPlatform.RaiseNotSupported(6);
                    return null;
            }
        }

        public static ConsoleBrush FromPosition(IBrush brush, int x, int y, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(brush);
            if (x < 0 || x > width)
                throw new ArgumentOutOfRangeException(nameof(x), "x is out bounds");
            if (y < 0 || y > height)
                throw new ArgumentOutOfRangeException(nameof(y), "y is out bounds");

            switch (brush)
            {
                case ILinearGradientBrush gradientBrush:
                {
                    if (width <= 0)
                        width = 1;
                    if (height <= 0)
                        height = 1;
                    // Calculate the relative position within the gradient
                    double horizontalRelativePosition = (double)x / width;
                    double verticalRelativePosition = (double)y / height;

                    // Interpolate horizontal and vertical colors
                    Color horizontalColor = InterpolateColor(gradientBrush, horizontalRelativePosition);
                    Color verticalColor = InterpolateColor(gradientBrush, verticalRelativePosition);

                    // Average the two colors to get the final color
                    Color color = BlendColors(horizontalColor, verticalColor);
                    return new ConsoleBrush(color);
                }
                case IRadialGradientBrush radialBrush:
                {
                    // Calculate the normalized center coordinates
                    double centerX = radialBrush.Center.Point.X * width;
                    double centerY = radialBrush.Center.Point.Y * height;

                    // Calculate the distance from the center
                    double dx = x - centerX;
                    double dy = y - centerY;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    // Normalize the distance based on the brush radius
                    double normalizedDistance = distance / (Math.Min(width, height) * radialBrush.Radius);

                    // Clamp the normalized distance to [0, 1]
                    normalizedDistance = Math.Min(Math.Max(normalizedDistance, 0), 1);

                    // Interpolate the color based on the normalized distance
                    Color color = InterpolateColor(radialBrush, normalizedDistance);
                    return new ConsoleBrush(color);
                }
                case IConicGradientBrush conicBrush:
                {
                    if (width <= 0)
                        width = 1;
                    if (height <= 0)
                        height = 1;
                    // Calculate the relative position within the gradient
                    double horizontalRelativePosition = (double)x / width;
                    double verticalRelativePosition = (double)y / height;

                    // Interpolate horizontal and vertical colors
                    Color horizontalColor = InterpolateColor(conicBrush, horizontalRelativePosition);
                    Color verticalColor = InterpolateColor(conicBrush, verticalRelativePosition);

                    // Average the two colors to get the final color
                    Color color = BlendColors(horizontalColor, verticalColor);
                    return new ConsoleBrush(color);
                }

                default:
                    return FromBrush(brush);
            }
        }

        // ReSharper disable once UnusedMember.Global used by Avalonia
        // ReSharper disable once UnusedParameter.Global
        public IBrush ProvideValue(IServiceProvider _)
        {
            return this;
        }

        private static Color InterpolateColor(IGradientBrush brush, double relativePosition)
        {
            IGradientStop before = null;
            IGradientStop after = null;

            foreach (IGradientStop stop in brush.GradientStops)
                if (stop.Offset <= relativePosition)
                {
                    before = stop;
                }
                else if (stop.Offset >= relativePosition)
                {
                    after = stop;
                    break;
                }

            if (before == null && after == null)
                throw new ArgumentException("no gradientstops defined");

            if (before == null) return after.Color;
            if (after == null) return before.Color;

            double ratio = (relativePosition - before.Offset) / (after.Offset - before.Offset);
            byte r = (byte)(before.Color.R + ratio * (after.Color.R - before.Color.R));
            byte g = (byte)(before.Color.G + ratio * (after.Color.G - before.Color.G));
            byte b = (byte)(before.Color.B + ratio * (after.Color.B - before.Color.B));
            byte a = (byte)(before.Color.A + ratio * (after.Color.A - before.Color.A));

            return Color.FromArgb(a, r, g, b);
        }

        private static Color BlendColors(Color color1, Color color2)
        {
            int r = (color1.R + color2.R) / 2;
            int g = (color1.G + color2.G) / 2;
            int b = (color1.B + color2.B) / 2;
            int a = (color1.A + color2.A) / 2;
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }
    }
}