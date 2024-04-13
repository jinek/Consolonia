using System;
using Avalonia;
using Avalonia.Media;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Avalonia.Styling;

namespace Consolonia.Core.Drawing
{
    public class FourBitColorBrush : AvaloniaObject, IBrush
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
        public Brush ProvideValue(IServiceProvider _)
        {
            return this;
        }

        public double Opacity { get; }
        public ITransform Transform { get; }
        public RelativePoint TransformOrigin { get; }
    }
}