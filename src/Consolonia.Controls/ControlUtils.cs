using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia;
using Avalonia.Reactive;
using NeoSmart.Unicode;
using Wcwidth;

namespace Consolonia.Controls
{
    public static class ControlUtils
    {
        public static IDisposable SubscribeAction<TValue>(
            this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable,
            Action<AvaloniaPropertyChangedEventArgs<TValue>> action)
        {
            return observable.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TValue>>(action));
        }

        public static string GetStyledPropertyName([CallerMemberName] string propertyFullName = null)
        {
            return propertyFullName![..^8];
        }

        private static Lazy<IConsoleCapabilities> _capabilities = new Lazy<IConsoleCapabilities>(() => AvaloniaLocator.Current.GetService<IConsoleCapabilities>());

        /// <summary>
        ///     Measure text for actual width
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ushort MeasureText(this string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            var console = _capabilities.Value;
            bool supportsComplexEmoji = console == null || console.SupportsComplexEmoji;

            ushort width = 0;
            ushort lastWidth = 0;
            foreach (Rune rune in text.EnumerateRunes())
            {
                var runeWidth = UnicodeCalculator.GetWidth(rune);
                if (runeWidth >= 0)
                {

                    if (supportsComplexEmoji &&
                        (rune.Value == Emoji.ZeroWidthJoiner || rune.Value == Emoji.ObjectReplacementCharacter))
                        width -= lastWidth;
                    else
                        width += (ushort)runeWidth;

                    if (runeWidth > 0)
                        lastWidth = (ushort)runeWidth;
                }
                // Control chars return as width < 0
                else
                {
                    if (rune.Value == 0x9 /* tab */)
                    {
                        // Avalonia uses hard coded 4 spaces for tabs (NOT column based tabstops), this may change in the future
                        width += 4;
                        lastWidth = 4;
                    }
                }
            }
            return width;
        }
    }
}