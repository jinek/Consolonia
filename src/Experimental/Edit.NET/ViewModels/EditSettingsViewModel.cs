using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using EditNET.DataModels;
using TextMateSharp.Grammars;

namespace EditNET.ViewModels
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

        public Settings ToSettings()
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
