using Avalonia.Controls;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryTabControl : UserControl
    {
        public GalleryTabControl()
        {
            InitializeComponent();

            DataContext = new PageViewModel
            {
                Tabs = new[]
                {
                    new TabItemViewModel
                    {
                        Header = "Arch",
                        Text = "This is the first templated tab page."
                    },
                    new TabItemViewModel
                    {
                        Header = "Leaf",
                        Text = "This is the second templated tab page."
                    },
                    new TabItemViewModel
                    {
                        Header = "Disabled",
                        Text = "You should not see this.",
                        IsEnabled = false
                    }
                },
                TabPlacement = Dock.Top
            };
        }
    }

    public class PageViewModel : ViewModelBase
    {
        private Dock _tabPlacement;

#pragma warning disable CA1819 // Properties should not return arrays
        public TabItemViewModel[] Tabs { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        public Dock TabPlacement
        {
            get => _tabPlacement;
            set => RaiseAndSetIfChanged(ref _tabPlacement, value);
        }
    }

    public class TabItemViewModel
    {
        public string Header { get; set; }
        public string Text { get; set; }
        public bool IsEnabled { get; init; } = true;
    }
}