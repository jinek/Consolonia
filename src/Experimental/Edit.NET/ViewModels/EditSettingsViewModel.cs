using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Edit.NET.DataModels;

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

        public EditSettingsViewModel(Settings settings)
        {
            // Initialize properties from current settings
            Theme = settings.ConsoloniaTheme.ToString();
            LightVariant = settings.LightVariant;
            ShowTabs = settings.ShowTabs;
            ShowSpaces = settings.ShowSpaces;
            DefaultExtension = settings.DefaultExtension;
        }

        public AppViewModel AppViewModel = (AppViewModel)App.Current.DataContext;

        [ObservableProperty] private string _theme;
        [ObservableProperty] private bool _lightVariant;
        [ObservableProperty] private bool _showTabs;
        [ObservableProperty] private bool _showSpaces;


        [RegularExpression(@"^(?i)\.[a-z0-9]+$", ErrorMessage = "Invalid extension")]
        [ObservableProperty] 
        private string _defaultExtension = ".txt";
    }
}
