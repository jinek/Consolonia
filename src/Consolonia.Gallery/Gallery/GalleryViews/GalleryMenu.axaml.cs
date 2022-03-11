using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    public class GalleryMenu : UserControl
    {
        private MenuPageViewModel _model;

        public GalleryMenu()
        {
            InitializeComponent();
            DataContext = new MenuPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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