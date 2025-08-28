using System;
using Avalonia.Markup.Xaml;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;

// ReSharper disable once CheckNamespace
namespace Consolonia
{
    //todo: define xaml namespace for out classes
    /// <summary>
    ///     Avalonia axaml extension which consumes ConsoleColor and produces AvaloniaColor
    /// </summary>
    public class EgaColorExtension : MarkupExtension
    {
        public EgaColorExtension()
        {
            Color = ConsoleColor.Black;
        }

        public EgaColorExtension(ConsoleColor color)
        {
            Color = color;
        }

        public EgaColorExtension(EgaColorMode mode)
        {
            Color = ConsoleColor.Black;
            Mode = mode;
        }

        public ConsoleColor Color { get; set; }
        public EgaColorMode Mode { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return EgaConsoleColorMode.ConvertToAvaloniaColor(Color, Mode);
        }
    }
}