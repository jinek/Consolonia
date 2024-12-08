using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Media;
using Consolonia.Core.Drawing;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryColors : UserControl
    {
        public GalleryColors()
        {
            InitializeComponent();
            var rnd = Random.Shared;
            this.DataContext = new RGBModel() { Color = Color.FromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255)) };
        }

        private void Random_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Random rnd = new Random();
            var model = (RGBModel)this.DataContext;
            model.Color = Color.FromRgb((byte)rnd.Next(255), (byte)rnd.Next(255), (byte)rnd.Next(255));
        }

        private void Red_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Random rnd = new Random();
            var model = (RGBModel)this.DataContext;
            model.Color = Color.FromRgb((byte)rnd.Next(255), model.Color.G, model.Color.B);
        }
        private void Green_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Random rnd = new Random();
            var model = (RGBModel)this.DataContext;
            model.Color = Color.FromRgb(model.Color.R, (byte)rnd.Next(255), model.Color.B);
        }
        private void Blue_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Random rnd = new Random();
            var model = (RGBModel)this.DataContext;
            model.Color = Color.FromRgb(model.Color.R, model.Color.G, (byte)rnd.Next(255));
        }
    }

    public class RGBModel : INotifyPropertyChanged
    {
        public RGBModel()
        {
        }

        private Color _color;
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
                Brush = new ConsoleBrush(_color);
            }
        }

        private ConsoleBrush _brush;
        public ConsoleBrush Brush
        {
            get => _brush; set
            {
                _brush = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Brush)));
            }
        }

        public ObservableCollection<Swatch> Swatches { get; } = new ObservableCollection<Swatch>();

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Swatch
    {
        public string Name { get; set; }

        public Color Color { get; set; }
    }
}