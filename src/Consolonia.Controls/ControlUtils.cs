using System;
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

        /// <summary>
        ///     Measure text for actual width
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static ushort MeasureText(this string text)
        {
            var console = AvaloniaLocator.Current.GetService<IConsoleCapabilities>();
            bool supportsComplexEmoji = console == null || console.SupportsComplexEmoji;

            ushort width = 0;
            ushort lastWidth = 0;
            foreach (Rune rune in text.EnumerateRunes())
            {
                ushort runeWidth = (ushort)UnicodeCalculator.GetWidth(rune);
                if (supportsComplexEmoji &&
                    (rune.Value == Emoji.ZeroWidthJoiner || rune.Value == Emoji.ObjectReplacementCharacter))
                    width -= lastWidth;
                else
                    width += runeWidth;

                if (runeWidth > 0)
                    lastWidth = runeWidth;
            }

            return width;
        }
    }
}