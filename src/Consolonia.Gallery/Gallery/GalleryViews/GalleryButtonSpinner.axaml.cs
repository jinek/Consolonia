using System;
using Avalonia.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryButtonSpinner : UserControl
    {
        private readonly string[] _mountains = new[]
        {
            "Everest",
            "K2 (Mount Godwin Austen)",
            "Kangchenjunga",
            "Lhotse",
            "Makalu",
            "Cho Oyu",
            "Dhaulagiri",
            "Manaslu",
            "Nanga Parbat",
            "Annapurna"
        };

        public GalleryButtonSpinner()
        {
            InitializeComponent();
        }

        public void OnSpin(object sender, SpinEventArgs e)
        {
            var spinner = (ButtonSpinner)sender;

            if (spinner.Content is TextBlock txtBox)
            {
                int value = Array.IndexOf(_mountains, txtBox.Text);
                if (e.Direction == SpinDirection.Increase)
                    value++;
                else
                    value--;

                if (value < 0)
                    value = _mountains.Length - 1;
                else if (value >= _mountains.Length)
                    value = 0;

                txtBox.Text = _mountains[value];
            }
        }
    }
}