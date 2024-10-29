using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Drawing
{
    [TypeConverter(typeof(ConsoleColorBrushConverter))]
    public class ConsoleColorBrush : AvaloniaObject, IImmutableBrush
    {
        public static readonly StyledProperty<Color> ColorProperty =
            AvaloniaProperty.Register<ConsoleColorBrush, Color>(nameof(Color));

        public static readonly StyledProperty<PixelBackgroundMode> ModeProperty =
            AvaloniaProperty.Register<ConsoleColorBrush, PixelBackgroundMode>(nameof(Mode));

        // ReSharper disable once UnusedMember.Global
        public ConsoleColorBrush(Color color, PixelBackgroundMode mode) : this(color)
        {
            Mode = mode;
        }

        public ConsoleColorBrush(Color color)
        {
            Color = color;
        }

        public ConsoleColorBrush()
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

    public class ConsoleColorBrushConverter : TypeConverter 
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