using System;
using Avalonia;
using Avalonia.Interactivity;
using Consolonia.Core.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class SomeDialogWindow : DialogWindow
    {
        internal const string DialogTitle = "Dialog popup";

        private static readonly Random Random = new();

        public SomeDialogWindow(double width, double height)
        {
            InitializeComponent();
            Title = DialogTitle;
            Width = width;
            Height = height;

            AttachedToVisualTree += OnShowDialog;
        }

        // ReSharper disable once MemberCanBePrivate.Global Can be used by constructor
        public SomeDialogWindow() : this(10 + Random.Next(20), 10 + Random.Next(10))
        {
        }

        private void OnShowDialog(object sender, VisualTreeAttachmentEventArgs e)
        {
            AttachedToVisualTree -= OnShowDialog;
            OneMoreButton.AttachedToVisualTree += OnButtonAttached;
        }

        private void OnButtonAttached(object sender, VisualTreeAttachmentEventArgs e)
        {
            OneMoreButton.AttachedToVisualTree -= OnButtonAttached;
            OneMoreButton.Focus();
        }

        private async void OneMore_Clicked(object sender, RoutedEventArgs e)
        {
            await new SomeDialogWindow().ShowDialogAsync(this);
        }
    }
}