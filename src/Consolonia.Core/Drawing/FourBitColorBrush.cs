using System;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Drawing
{
    [TypeConverter(typeof(FourBitBrushConverter))]
    public class FourBitColorBrush : AvaloniaObject, IImmutableBrush
    {
        public static readonly StyledProperty<ConsoleColor> ColorProperty =
            AvaloniaProperty.Register<FourBitColorBrush, ConsoleColor>(nameof(Color));

        public static readonly StyledProperty<PixelBackgroundMode> ModeProperty =
            AvaloniaProperty.Register<FourBitColorBrush, PixelBackgroundMode>(nameof(Mode));

        // ReSharper disable once UnusedMember.Global
        public FourBitColorBrush(ConsoleColor consoleColor, PixelBackgroundMode mode) : this(consoleColor)
        {
            Mode = mode;
        }

        public FourBitColorBrush(ConsoleColor consoleColor)
        {
            Color = consoleColor;
        }

        public FourBitColorBrush()
        {
        }

        public PixelBackgroundMode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public ConsoleColor Color
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

        public double Opacity => 0;
        public ITransform Transform => null;
        public RelativePoint TransformOrigin => RelativePoint.TopLeft;
    }

    public class FourBitBrushConverter : TypeConverter 
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
                return Enum.TryParse(s, out ConsoleColor result) 
                    ? result 
                    : null;
            }
    
            return null;
        }
    }
    
    
}