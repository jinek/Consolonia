using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Consolonia.Core.Drawing;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(1000)]
    public partial class GalleryImage: UserControl
    {
        public GalleryImage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}