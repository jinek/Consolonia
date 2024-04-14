using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Reactive;

namespace Consolonia.Core.Helpers
{
    public static class Extensions
    {
        public static void SubscribeAction<TValue>(
            this IObservable<AvaloniaPropertyChangedEventArgs<TValue>> observable,
            Action<AvaloniaPropertyChangedEventArgs<TValue>> action)
        {
            observable.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TValue>>(action));
        }
    }
}