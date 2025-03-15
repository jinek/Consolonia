using System;
using System.IO;
using Avalonia;
using Avalonia.Metadata;
using AvaloniaMarkup = Avalonia.Markup.Xaml.MarkupExtensions;

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

    public sealed class
        OnPlatformExtension<TReturn> : OnPlatformConsoleExtensionBase<TReturn, AvaloniaMarkup.On<TReturn>>
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

    public abstract class
        OnPlatformConsoleExtensionBase<TReturn, TOn> : AvaloniaMarkup.OnPlatformExtensionBase<TReturn, TOn>
        where TOn : AvaloniaMarkup.On<TReturn>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        [MarkupExtensionOption("CONSOLE")] public TReturn Console { get; set; }

        private protected static bool ShouldProvideOptionInternal2(string option)
        {
            return option switch
            {
                "CONSOLE" => IsConsole(),
                _ => AvaloniaMarkup.OnPlatformExtension.ShouldProvideOption(option)
            };
        }

        private static bool? _isConsole;
        private static bool IsConsole()
        {
            if (_isConsole.HasValue)
                return _isConsole.Value;

            if (Application.Current?.ApplicationLifetime != null)
            {
                _isConsole = Application.Current.ApplicationLifetime.GetType().Name == "ConsoloniaLifetime";
                return _isConsole.Value;
            }

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    _isConsole = System.Console.WindowHeight > 0;
                }
                catch (IOException)
                {
                    _isConsole = false;
                }
                return _isConsole.Value;
            }

            // This works on Unix systems
            _isConsole = !(System.Console.IsInputRedirected &&
                               System.Console.IsOutputRedirected &&
                               System.Console.IsErrorRedirected);
            return _isConsole.Value;
        }
    }
}