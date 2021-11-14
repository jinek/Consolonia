using System;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBuffer;

namespace Consolonia.Core.Drawing
{
    public class FourBitColorBrush : Brush
    {
        public static readonly StyledProperty<ConsoleColor> ColorProperty =
            AvaloniaProperty.Register<FourBitColorBrush, ConsoleColor>(nameof(Color));

        public static readonly StyledProperty<PixelBackgroundMode> ModeProperty =
            AvaloniaProperty.Register<FourBitColorBrush, PixelBackgroundMode>(nameof(Mode));

        static FourBitColorBrush()
        {
            AffectsRender<FourBitColorBrush>(ColorProperty);
            AffectsRender<FourBitColorBrush>(ModeProperty);
        }

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
        public Brush ProvideValue(IServiceProvider _)
        {
            return this;
        }


        public override IBrush ToImmutable()
        {
            //todo: implement immutable
            return new FourBitColorBrush { Color = Color, Mode = Mode };
        }
    }
}