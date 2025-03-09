using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Iciclecreek.Avalonia.WindowManager;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryWindows : UserControl
    {
        public GalleryWindows()
        {
            InitializeComponent();
        }

        private void OnShowWindowClick(object? sender, RoutedEventArgs e)
        {
            var window = new MyWindow()
            {
                WindowStartupLocation = Enum.Parse<WindowStartupLocation>((StartupLocationCombo.SelectedItem as ComboBoxItem).Tag.ToString()),
                SizeToContent = Enum.Parse<SizeToContent>(((ComboBoxItem)SizeToContentCombo.SelectedItem).Tag.ToString()),
                WindowState = Enum.Parse<WindowState>(((ComboBoxItem)WindowStateCombo.SelectedItem).Tag.ToString()),    
            };

            window.SizeToBounds(this.Bounds);

            window.Show();
        }

    }
}