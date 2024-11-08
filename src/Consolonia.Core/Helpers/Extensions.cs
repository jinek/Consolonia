using System;
using Avalonia;
using Avalonia.Reactive;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

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

        public static void Print(this IConsole console, PixelBufferCoordinate point, Pixel pixel)
        {
            console.Print(point, 
                pixel.Background.Color, 
                pixel.Foreground.Color, 
                pixel.Foreground.Style, 
                pixel.Foreground.Weight, 
                pixel.Foreground.TextDecorations, 
                pixel.Foreground.Symbol.Text);
        }
    }
}