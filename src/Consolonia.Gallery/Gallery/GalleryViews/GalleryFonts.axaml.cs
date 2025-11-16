using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    public partial class GalleryFonts : UserControl
    {
        public GalleryFonts()
        {
            InitializeComponent();
            var fonts = new FontsViewModel();
            DataContext = fonts;
        }
    }

    [ObservableObject]
    public partial class FontsViewModel
    {
        public List<FontStyle> FontStyles = new()
        {
            FontStyle.Normal,
            FontStyle.Italic,
            FontStyle.Oblique
        };

        public List<FontWeight> FontWeights = new()
        {
            FontWeight.Thin,
            FontWeight.ExtraLight,
            FontWeight.Light,
            FontWeight.Normal,
            FontWeight.Medium,
            FontWeight.SemiBold,
            FontWeight.Bold,
            FontWeight.ExtraBold,
            FontWeight.Black,
            FontWeight.Heavy
        };

        [ObservableProperty] private ObservableCollection<FontViewModel> _fonts = new();

        [ObservableProperty] private FontViewModel _selectedFont;

        [ObservableProperty] private FontStyle _selectedFontStyle = FontStyle.Normal;

        [ObservableProperty] private FontWeight _selectedFontWeight = FontWeight.Normal;

        public FontsViewModel()
        {
            Fonts.Add(new FontViewModel
                { Font = "ConsoleDefault", FontFamily = FontFamily.Parse("ConsoleDefault"), FontSize = 1 });
            Fonts.Add(new FontViewModel("WideTerm", 1));
            Fonts.Add(new FontViewModel("Braille", 2));
            Fonts.Add(new FontViewModel("Circle", 1));
            Fonts.Add(new FontViewModel("Cursive", 6));
            Fonts.Add(new FontViewModel("Digital", 3));
            Fonts.Add(new FontViewModel("Doom", 8));
            Fonts.Add(new FontViewModel("Graffiti", 6));
            Fonts.Add(new FontViewModel("Mini", 4));
            Fonts.Add(new FontViewModel("Rectangles", 6));
            Fonts.Add(new FontViewModel("Slant", 6));
            Fonts.Add(new FontViewModel("Emboss", 3));
            Fonts.Add(new FontViewModel("Point", 3));
            Fonts.Add(new FontViewModel("Straight", 4));
            Fonts.Add(new FontViewModel("Pepper", 4));
            Fonts.Add(new FontViewModel("Standard", 5));
            Fonts.Add(new FontViewModel("Standard", 6));
            Fonts.Add(new FontViewModel("Standard", 8));
            Fonts.Add(new FontViewModel("Mono", 8));
            Fonts.Add(new FontViewModel("Mono", 10));
            SelectedFont = Fonts.First(f => f.Font == "ConsoleDefault");
        }
    }

    [ObservableObject]
    public partial class FontViewModel
    {
        [ObservableProperty] [NotifyPropertyChangedFor(nameof(DisplayName))]
        private string _font;

        [ObservableProperty] private FontFamily _fontFamily;

        [ObservableProperty] [NotifyPropertyChangedFor(nameof(DisplayName))]
        private int _fontSize;

        [ObservableProperty] private FontStyle _style;

        [ObservableProperty] private FontWeight _weight;

        public FontViewModel()
        {
        }

        public FontViewModel(string fontFamily, int fontSize)
        {
            Font = fontFamily;
            FontSize = fontSize;
            FontFamily = FontFamily.Parse($"fonts:Consolonia#{fontFamily}");
        }

        public string DisplayName => $"{Font} [{FontSize}]";
    }
}