using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Edit.NET.DataModels;
using TextMateSharp.Grammars;

namespace Edit.NET.ViewModels
{
    public partial class EditSettingsViewModel : ObservableValidator
    {
        public ObservableCollection<string> AvailableThemes { get; } = new()
        {
            "Modern",
            "ModernContrast",
            "TurboVision",
            "TurboVisionCompatible",
            "TurboVisionGray",
            "TurboVisionElegant"
        };

        public ObservableCollection<string> SyntaxThemes { get; } = new();

        public EditSettingsViewModel(Settings settings)
        {
            // Initialize properties from current settings
            SyntaxThemes = new ObservableCollection<string>(Enum.GetNames<ThemeName>());
            Theme = settings.ConsoloniaTheme.ToString();
            LightVariant = settings.LightVariant;
            ShowTabs = settings.ShowTabs;
            ShowSpaces = settings.ShowSpaces;
            SelectedSyntaxTheme = settings.SyntaxTheme ?? ThemeName.VisualStudioDark.ToString();
            DefaultExtension = settings.DefaultExtension;
        }

        public Settings GetSettings()
        {
            return new Settings
            {
                ConsoloniaTheme = this.Theme,
                LightVariant = this.LightVariant,
                ShowTabs = this.ShowTabs,
                ShowSpaces = this.ShowSpaces,
                DefaultExtension = this.DefaultExtension,
                SyntaxTheme = this.SelectedSyntaxTheme
            };
        }

        public AppViewModel AppViewModel = (AppViewModel)App.Current.DataContext;

        [ObservableProperty] private string _theme;
        [ObservableProperty] private bool _lightVariant;
        [ObservableProperty] private bool _showTabs;
        [ObservableProperty] private bool _showSpaces;


        [RegularExpression(@"^(?i)\.[a-z0-9]+$", ErrorMessage = "Invalid extension")]
        [ObservableProperty]
        private string _defaultExtension = ".txt";

        [ObservableProperty]
        private string _selectedSyntaxTheme;
    }
}
