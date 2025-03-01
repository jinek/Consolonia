using Avalonia;
using AvaloniaMarkup =Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;

namespace Consolonia.Controls.Markup
{
    public sealed class OnPlatformExtension : OnPlatformConsoleExtensionBase<object, AvaloniaMarkup.On>
    {
        public OnPlatformExtension()
        {
        }

        public OnPlatformExtension(object defaultValue)
        {
            Default = defaultValue;
        }

        public static bool ShouldProvideOption(string option)
        {
            return ShouldProvideOptionInternal2(option);
        }
    }

    public sealed class OnPlatformExtension<TReturn> : OnPlatformConsoleExtensionBase<TReturn, AvaloniaMarkup.On<TReturn>>
    {
        public OnPlatformExtension()
        {
        }

        public OnPlatformExtension(TReturn defaultValue)
        {
            Default = defaultValue;
        }

#pragma warning disable CA1000 // Do not declare static members on generic types
        public static bool ShouldProvideOption(string option)
#pragma warning restore CA1000 // Do not declare static members on generic types
        {
            return ShouldProvideOptionInternal2(option);
        }
    }

    public abstract class OnPlatformConsoleExtensionBase<TReturn, TOn> : AvaloniaMarkup.OnPlatformExtensionBase<TReturn, TOn>
        where TOn : AvaloniaMarkup.On<TReturn>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [MarkupExtensionOption("CONSOLE")] public TReturn Console { get; set; }

        private protected static bool ShouldProvideOptionInternal2(string option)
        {
            return option switch
            {
                "CONSOLE" => Application.Current.ApplicationLifetime?.GetType().Name == "ConsoloniaLifetime",
                _ => AvaloniaMarkup.OnPlatformExtension.ShouldProvideOption(option)
            };
        }
    }
}
