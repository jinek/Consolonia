using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryGradientBrush : UserControl
    {
        public GalleryGradientBrush()
        {
            InitializeComponent();
        }

        private void Linear_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops
                {
                    new GradientStop { Color = Colors.White, Offset = 0 },
                    new GradientStop { Color = Colors.Red, Offset = 1 }
                }
            };
        }

        private void Radial_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.Background = new RadialGradientBrush
            {
                Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                GradientOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                RadiusX = RelativeScalar.Parse("50%"),
                RadiusY = RelativeScalar.Parse("50%"),
                GradientStops = new GradientStops
                {
                    new GradientStop { Color = Colors.Black, Offset = 0 },
                    new GradientStop { Color = Colors.Red, Offset = 1 }
                }
            };
        }

        private void Conic_Click(object sender, RoutedEventArgs e)
        {
            MyGrid.Background = new ConicGradientBrush
            {
                Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                Angle = 0,
                GradientStops = new GradientStops
                {
                    new GradientStop { Color = Colors.Red, Offset = 0 },
                    new GradientStop { Color = Colors.Yellow, Offset = 0.25 },
                    new GradientStop { Color = Colors.Green, Offset = 0.5 },
                    new GradientStop { Color = Colors.Cyan, Offset = 0.75 },
                    new GradientStop { Color = Colors.Blue, Offset = 1 }
                }
            };
        }
    }
}