using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Controls;

namespace Sandbox
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FontsViewModel fonts = new FontsViewModel();
            this.DataContext = fonts;
            this.Fonts.SelectedIndex = 0;
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            var lifetime = Application.Current!.ApplicationLifetime as IControlledApplicationLifetime;
            lifetime!.Shutdown();
        }
    }

    [ObservableObject]
    public partial class FontsViewModel
    {
        [ObservableProperty]
        private ObservableCollection<FontViewModel> _fonts = new();

        [ObservableProperty]
        private FontViewModel _selectedFont;

        public FontsViewModel()
        {
            Fonts.Add(new FontViewModel() { Font = "ConsoleDefault", FontFamily = FontFamily.Parse("ConsoleDefault"), FontSize = 1 });
            Fonts.Add(new FontViewModel("WideTerm", 1));
            Fonts.Add(new FontViewModel("Braille", 2));
            Fonts.Add(new FontViewModel("Circle", 1));
            Fonts.Add(new FontViewModel("Benjamin", 1));
            Fonts.Add(new FontViewModel("BigFig", 3));
            Fonts.Add(new FontViewModel("Contessa", 4));
            Fonts.Add(new FontViewModel("Cursive", 6));
            Fonts.Add(new FontViewModel("Cygnet", 5));
            Fonts.Add(new FontViewModel("Digital", 3));
            Fonts.Add(new FontViewModel("Doom", 8));
            Fonts.Add(new FontViewModel("Graffiti", 6));
            Fonts.Add(new FontViewModel("Mini", 4));
            Fonts.Add(new FontViewModel("Rectangles", 6));
            Fonts.Add(new FontViewModel("Short", 3));
            Fonts.Add(new FontViewModel("Slant", 6));
            Fonts.Add(new FontViewModel("Emboss", 3));
            Fonts.Add(new FontViewModel("Point", 2));
            Fonts.Add(new FontViewModel("Point", 3));
            Fonts.Add(new FontViewModel("Standard", 5));
            Fonts.Add(new FontViewModel("Standard", 6));
            Fonts.Add(new FontViewModel("Standard", 8));
            Fonts.Add(new FontViewModel("Mono", 9));
            Fonts.Add(new FontViewModel("Mono", 12));
            Fonts.Add(new FontViewModel("SmallMono", 9));
            Fonts.Add(new FontViewModel("SmallMono", 12));
            SelectedFont = Fonts.First(f => f.Font == "ConsoleDefault");
        }
    }

    [ObservableObject]
    public partial class FontViewModel
    {
        public FontViewModel()
        {
        }

        public FontViewModel(string fontFamily, int fontSize)
        {
            Font = fontFamily;
            FontSize = fontSize;
            FontFamily = FontFamily.Parse($"fonts:Consolonia#{fontFamily}");
        }

        private string DisplayName => $"{Font} [{FontSize}]";

        [ObservableProperty]
        private string _font;

        [ObservableProperty]
        private FontFamily _fontFamily;

        [ObservableProperty]
        private int _fontSize;
    }
}