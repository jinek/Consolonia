using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    public class GalleryTabControl : UserControl
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class PageViewModel : ViewModelBase
    {
        private Dock _tabPlacement;

        public TabItemViewModel[] Tabs { get; set; }

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