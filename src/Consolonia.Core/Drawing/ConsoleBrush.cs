using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Drawing
{
    [TypeConverter(typeof(ConsoleBrushConverter))]
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

        /// <summary>
        /// Convert a IBrush to a Brush. 
        /// </summary>
        /// <remarks>
        /// NOTE: If it's a ConsoleBrush it will be passed through unchanged, unless mode is set then it will convert consolebrush to mode
        /// </remarks>
        /// <param name="brush"></param>
        /// <param name="mode">Default is Colored.</param>
        /// <returns></returns>
        public static ConsoleBrush FromBrush(IBrush brush, PixelBackgroundMode? mode = null)
        {
            switch (brush)
            {
                case ConsoleBrush consoleBrush:
                    if (mode != null && consoleBrush.Mode != mode)
                    {
                        return new ConsoleBrush(consoleBrush.Color, mode.Value);
                    }
                    return consoleBrush;
                case LineBrush lineBrush:
                    switch (lineBrush.Brush)
                    {
                        case ConsoleBrush:
                            return lineBrush.Brush as ConsoleBrush;
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

        // ReSharper disable once UnusedMember.Global used by Avalonia
        // ReSharper disable once UnusedParameter.Global
        public IBrush ProvideValue(IServiceProvider _)
        {
            return this;
        }

        public double Opacity => 1;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;
    }

    public class ConsoleBrushConverter : TypeConverter 
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(
            ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                return Enum.TryParse(s, out Color result) 
                    ? result 
                    : null;
            }
    
            return null;
        }
    }
    
    
}