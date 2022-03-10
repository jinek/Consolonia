using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ControlCatalog.ViewModels;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class GalleryMenu : UserControl
    {
        public GalleryMenu()
        {
            InitializeComponent();
            DataContext = new MenuPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private MenuPageViewModel _model;
        protected override void OnDataContextChanged(EventArgs e)
        {
            if (_model != null)
                _model.View = null;
            _model = DataContext as MenuPageViewModel;
            if (_model != null)
                _model.View = this;

            base.OnDataContextChanged(e);
        }
    }
}