using System;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Consolonia.Themes.TurboVision.Templates.Controls.Dialog;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class SomeDialogWindow : DialogWindow
    {
        private static readonly Random Random = new();

        public SomeDialogWindow()
        {
            InitializeComponent();
            Width = 10 + Random.Next(20);
            Height = 10 + Random.Next(10);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OneMore_Clicked(object sender, RoutedEventArgs e)
        {
            await new SomeDialogWindow().ShowDialogAsync(this);
        }
    }
}