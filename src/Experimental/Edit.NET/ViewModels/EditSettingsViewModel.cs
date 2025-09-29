using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Styling;
using Edit.NET.DataModels;

namespace Edit.NET
{
    public partial class EditSettingsViewModel : ObservableObject
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
        [ObservableProperty] private string _defaultExtension;
    }
}
