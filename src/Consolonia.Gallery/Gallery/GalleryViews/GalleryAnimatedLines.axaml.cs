using System;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Consolonia.Core.Infrastructure;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    public class GalleryAnimatedLines : UserControl
    {
        public GalleryAnimatedLines()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void PauseButton_OnClick(object _, RoutedEventArgs _2)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            await ((ConsoloniaLifetime)Application.Current!.ApplicationLifetime!).DisconnectFromConsoleAsync(cts.Token);
            Console.ResetColor();
            Console.Clear();
        }
    }
}