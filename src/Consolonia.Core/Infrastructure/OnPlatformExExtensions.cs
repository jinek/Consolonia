using System;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;

// ReSharper disable CheckNamespace
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Consolonia
{
    public sealed class OnPlatformExExtension : OnPlatformConsoleExtensionBase<object, On>
    {
        public OnPlatformExExtension()
        {
        }

        public OnPlatformExExtension(object defaultValue)
        {
            Default = defaultValue;
        }

        public static bool ShouldProvideOption(string option)
        {
            return ShouldProvideOptionInternal2(option);
        }
    }

    public sealed class OnPlatformExExtension<TReturn> : OnPlatformConsoleExtensionBase<TReturn, On<TReturn>>
    {
        public OnPlatformExExtension()
        {
        }

        public OnPlatformExExtension(TReturn defaultValue)
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

    public abstract class OnPlatformConsoleExtensionBase<TReturn, TOn> : OnPlatformExtensionBase<TReturn, TOn>
        where TOn : On<TReturn>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [MarkupExtensionOption("CONSOLE")] public TReturn Console { get; set; }

        private protected static bool ShouldProvideOptionInternal2(string option)
        {
            return option switch
            {
                "CONSOLE" => Application.Current.ApplicationLifetime is ConsoloniaLifetime,
                _ => OnPlatformExtension.ShouldProvideOption(option)
            };
        }
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure