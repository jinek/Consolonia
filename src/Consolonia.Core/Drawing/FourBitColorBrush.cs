using System;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBuffer;

namespace Consolonia.Core.Drawing
{
    public class FourBitColorBrush : Brush
    {
        static FourBitColorBrush()
        {
            AffectsRender<FourBitColorBrush>(ColorProperty);
            AffectsRender<FourBitColorBrush>(ModeProperty);
        }

        public static readonly StyledProperty<ConsoleColor> ColorProperty =
            AvaloniaProperty.Register<FourBitColorBrush, ConsoleColor>(nameof(Color));

        public static readonly StyledProperty<PixelBackgroundMode> ModeProperty =
            AvaloniaProperty.Register<FourBitColorBrush, PixelBackgroundMode>(nameof(Mode));

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

        public override IBrush ToImmutable()
        {
            return new FourBitColorBrush { Color = Color, Mode = Mode };
        }
    }
}