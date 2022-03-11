using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public class GalleryScrollViewer : UserControl
    {
        public GalleryScrollViewer()
        {
            InitializeComponent();

            DataContext = new ScrollViewerPageViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class ScrollViewerPageViewModel : ViewModelBase
    {
        private bool _allowAutoHide;
        private ScrollBarVisibility _horizontalScrollVisibility;
        private ScrollBarVisibility _verticalScrollVisibility;

        public ScrollViewerPageViewModel()
        {
            AvailableVisibility = new List<ScrollBarVisibility>
            {
                ScrollBarVisibility.Auto,
                ScrollBarVisibility.Visible,
                ScrollBarVisibility.Hidden,
                ScrollBarVisibility.Disabled
            };

            HorizontalScrollVisibility = ScrollBarVisibility.Auto;
            VerticalScrollVisibility = ScrollBarVisibility.Auto;
            AllowAutoHide = true;
        }

        public bool AllowAutoHide
        {
            get => _allowAutoHide;
            set => RaiseAndSetIfChanged(ref _allowAutoHide, value);
        }

        public ScrollBarVisibility HorizontalScrollVisibility
        {
            get => _horizontalScrollVisibility;
            set => RaiseAndSetIfChanged(ref _horizontalScrollVisibility, value);
        }

        public ScrollBarVisibility VerticalScrollVisibility
        {
            get => _verticalScrollVisibility;
            set => RaiseAndSetIfChanged(ref _verticalScrollVisibility, value);
        }

        public List<ScrollBarVisibility> AvailableVisibility { get; }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                RaisePropertyChanged(propertyName);
                return true;
            }

            return false;
        }


        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}