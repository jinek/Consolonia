using System;
using Avalonia.Reactive;

namespace Consolonia.Core.Helpers
{
    /// <summary>
    ///     Provides common observable methods as a replacement for the Rx framework.
    /// </summary>
    /// <remarks>
    ///     COPIED FROM AVALONIA.BASE BECAUSE IT IS NOT PUBLICLY ACCESSIBLE,
    ///     it's either do this or pull in all of Rx framework as a dependency
    /// </remarks>
    internal static class Observable
    {
        public static IObservable<TSource> Create<TSource>(Func<IObserver<TSource>, IDisposable> subscribe)
        {
            return new CreateWithDisposableObservable<TSource>(subscribe);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> action)
        {
            return source.Subscribe(new AnonymousObserver<T>(action));
        }

        public static IObservable<TResult> Select<TSource, TResult>(this IObservable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return Create<TResult>(obs =>
            {
                return source.Subscribe(new AnonymousObserver<TSource>(
                    input =>
                    {
                        TResult value;
#pragma warning disable CA1031 // Do not catch general exception types
                        try
                        {
                            value = selector(input);
                        }
                        catch (Exception ex)
                        {
                            obs.OnError(ex);
                            return;
                        }
#pragma warning restore CA1031 // Do not catch general exception types

                        obs.OnNext(value);
                    }, obs.OnError, obs.OnCompleted));
            });
        }

        public static IObservable<TSource> StartWith<TSource>(this IObservable<TSource> source, TSource value)
        {
            return Create<TSource>(obs =>
            {
                obs.OnNext(value);
                return source.Subscribe(obs);
            });
        }

        public static IObservable<TSource> Where<TSource>(this IObservable<TSource> source,
            Func<TSource, bool> predicate)
        {
            return Create<TSource>(obs =>
            {
                return source.Subscribe(new AnonymousObserver<TSource>(
                    input =>
                    {
                        bool shouldRun;
#pragma warning disable CA1031 // Do not catch general exception types
                        try
                        {
                            shouldRun = predicate(input);
                        }
                        catch (Exception ex)
                        {
                            obs.OnError(ex);
                            return;
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                        if (shouldRun) obs.OnNext(input);
                    }, obs.OnError, obs.OnCompleted));
            });
        }

        public static IObservable<T> Skip<T>(this IObservable<T> source, int skipCount)
        {
            if (skipCount <= 0) throw new ArgumentException("Skip count must be bigger than zero", nameof(skipCount));

            return Create<T>(obs =>
            {
                int remaining = skipCount;
                return source.Subscribe(new AnonymousObserver<T>(
                    input =>
                    {
                        if (remaining <= 0)
                            obs.OnNext(input);
                        else
                            remaining--;
                    }, obs.OnError, obs.OnCompleted));
            });
        }


        private sealed class CreateWithDisposableObservable<TSource> : IObservable<TSource>
        {
            private readonly Func<IObserver<TSource>, IDisposable> _subscribe;

            public CreateWithDisposableObservable(Func<IObserver<TSource>, IDisposable> subscribe)
            {
                _subscribe = subscribe;
            }

            public IDisposable Subscribe(IObserver<TSource> observer)
            {
                return _subscribe(observer);
            }
        }
    }
}