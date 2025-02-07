using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryCarousel : UserControl
    {
        private readonly Carousel _carousel;
        private readonly Button _left;
        private readonly ComboBox _orientation;
        private readonly Button _right;
        private readonly ComboBox _transition;

        public GalleryCarousel()
        {
            InitializeComponent();
            _carousel = this.Get<Carousel>("carousel");
            _left = this.Get<Button>("left");
            _right = this.Get<Button>("right");
            _transition = this.Get<ComboBox>("transition");
            _orientation = this.Get<ComboBox>("orientation");
            _left.Click += (s, e) => _carousel.Previous();
            _right.Click += (s, e) => _carousel.Next();
            _transition.SelectionChanged += TransitionChanged;
            _orientation.SelectionChanged += TransitionChanged;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void TransitionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (_transition.SelectedIndex)
            {
                case 0:
                    _carousel.PageTransition = null;
                    break;
                case 1:
                    _carousel.PageTransition = new PageSlide(TimeSpan.FromSeconds(0.25),
                        _orientation.SelectedIndex == 0
                            ? PageSlide.SlideAxis.Horizontal
                            : PageSlide.SlideAxis.Vertical);
                    break;
                case 2:
                    _carousel.PageTransition = new CrossFade(TimeSpan.FromSeconds(0.25));
                    break;
                case 3:
                    _carousel.PageTransition = new Rotate3DTransition(TimeSpan.FromSeconds(0.5),
                        _orientation.SelectedIndex == 0
                            ? PageSlide.SlideAxis.Horizontal
                            : PageSlide.SlideAxis.Vertical);
                    break;
            }
        }
    }
}