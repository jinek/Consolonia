using System;
using System.Runtime.CompilerServices;
using Avalonia.Reactive;
using Avalonia;

namespace Consolonia.Controls
{
    public static class Utils
    {
        public static IDisposable SubscribeAction<TValue>(
                this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable,
                Action<AvaloniaPropertyChangedEventArgs<TValue>> action)
        {
            return observable.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TValue>>(action));
        }

        //public static bool IsNearlyEqual(this double value, double compareTo)
        //{
        //    //todo: strange implementation for this name
        //    return value.CompareTo(compareTo) == 0;
        //}

        public static string GetStyledPropertyName([CallerMemberName] string propertyFullName = null)
        {
            return propertyFullName![..^8];
        }
    }
}