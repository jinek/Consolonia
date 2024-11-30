using System;
using Avalonia;
using Avalonia.Interactivity;
using Consolonia.Core.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class SomeDialogWindow : DialogWindow
    {
        private static readonly Random Random = new();

        public SomeDialogWindow(double width, double height)
        {
            InitializeComponent();
            Width = width;
            Height = height;

            AttachedToVisualTree += OnShowDialog;
        }

        private void OnShowDialog(object sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            AttachedToVisualTree -= OnShowDialog;
            OneMoreButton.AttachedToVisualTree += OnButtonAttached;
        }

        private void OnButtonAttached(object? sender, VisualTreeAttachmentEventArgs e)
        {
            OneMoreButton.AttachedToVisualTree -= OnButtonAttached;
            OneMoreButton.Focus();
        }

        // ReSharper disable once MemberCanBePrivate.Global Can be used by constructor
        public SomeDialogWindow() : this(10 + Random.Next(20), 10 + Random.Next(10))
        {
        }

        // ReSharper disable UnusedParameter.Local
        private async void OneMore_Clicked(object sender, RoutedEventArgs e)
        // ReSharper restore UnusedParameter.Local
        {
            await new SomeDialogWindow().ShowDialogAsync(this);
        }
    }
}