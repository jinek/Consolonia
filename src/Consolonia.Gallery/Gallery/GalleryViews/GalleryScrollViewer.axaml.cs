using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
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
        private readonly bool _allowAutoHide;
        private readonly ScrollBarVisibility _horizontalScrollVisibility;
        private readonly ScrollBarVisibility _verticalScrollVisibility;

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
            init => RaiseAndSetIfChanged(ref _allowAutoHide, value);
        }

        public ScrollBarVisibility HorizontalScrollVisibility
        {
            get => _horizontalScrollVisibility;
            init => RaiseAndSetIfChanged(ref _horizontalScrollVisibility, value);
        }

        public ScrollBarVisibility VerticalScrollVisibility
        {
            get => _verticalScrollVisibility;
            init => RaiseAndSetIfChanged(ref _verticalScrollVisibility, value);
        }

        public List<ScrollBarVisibility> AvailableVisibility { get; }
    }

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // ReSharper disable once UnusedMethodReturnValue.Global
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