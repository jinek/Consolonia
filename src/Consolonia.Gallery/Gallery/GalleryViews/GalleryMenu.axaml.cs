using System;
using Avalonia.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryMenu : UserControl
    {
        private MenuPageViewModel _model;

        public GalleryMenu()
        {
            InitializeComponent();
            DataContext = new MenuPageViewModel();
        }

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