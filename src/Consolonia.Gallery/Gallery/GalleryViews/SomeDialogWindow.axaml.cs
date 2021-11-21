using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Consolonia.Themes.TurboVision.Templates.Controls.Dialog;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class SomeDialogWindow : UserControl
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

        private void OneMore_Clicked(object? sender, RoutedEventArgs e)
        {
            new SomeDialogWindow().ShowDialogPrivate(this);
        }
    }
}