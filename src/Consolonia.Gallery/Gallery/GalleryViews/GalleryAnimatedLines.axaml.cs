using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    // ReSharper disable once UnusedType.Global
    public partial class GalleryAnimatedLines : UserControl
    {
        public GalleryAnimatedLines()
        {
            InitializeComponent();
        }

        private async void PauseButton_OnClick(object _, RoutedEventArgs _2)
        // ReSharper restore UnusedParameter.Local
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            // ReSharper disable PossibleNullReferenceException //todo: build task does not understand ! operator
            await ((ConsoloniaLifetime)Application.Current!.ApplicationLifetime!).DisconnectFromConsoleAsync(cts.Token);
            // ReSharper restore PossibleNullReferenceException
            Console.ResetColor();
            Console.Clear();
        }


        // ReSharper disable UnusedParameter.Local
    }
}