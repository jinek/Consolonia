using System;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Consolonia.Themes.TurboVision.Templates.Controls.Dialog;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class SomeDialogWindow : DialogWindow
    {
        private static readonly Random Random = new();

        public SomeDialogWindow(double width, double height)
        {
            InitializeComponent();
            Width = width;
            Height = height;
        }
        
        public SomeDialogWindow() : this(10 + Random.Next(20),Random.Next(10))
        {

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // ReSharper disable UnusedParameter.Local
        private async void OneMore_Clicked(object sender, RoutedEventArgs e)
            // ReSharper restore UnusedParameter.Local
        {
            await new SomeDialogWindow().ShowDialogAsync(this);
        }
    }
}