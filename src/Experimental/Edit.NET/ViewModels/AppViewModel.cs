using System;
using System.IO;
using System.Text.Json;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Controls;
using Edit.NET.DataModels;
using TextMateSharp.Grammars;

namespace Edit.NET.ViewModels
{
    public partial class AppViewModel : ObservableObject
    {
        private static readonly string SettingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Edit.NET", "settings.json");

        public AppViewModel()
        {

        }

        [ObservableProperty]
        private ConsoloniaTheme _uITheme = NET.ConsoloniaTheme.Modern;

        [ObservableProperty]
        private ThemeVariant _uIThemeVariant = ThemeVariant.Dark;

        [ObservableProperty]
        private bool _showTabs = true;

        [ObservableProperty]
        private bool _showSpaces = true;

        [ObservableProperty]
        private string _defaultExtension = ".txt";

        [ObservableProperty]
        private ThemeName _syntaxTheme = ThemeName.VisualStudioDark;

        public Settings GetSettings()
        {
            return new Settings()
            {
                ConsoloniaTheme = this.UITheme.ToString(),
                LightVariant = this.UIThemeVariant == ThemeVariant.Light,
                ShowTabs = this.ShowTabs,
                ShowSpaces = this.ShowSpaces,
                SyntaxTheme = this.SyntaxTheme.ToString(),
                DefaultExtension = this.DefaultExtension
            };
        }

        public void SetSettings(Settings settings)
        {
            this.UITheme = Enum.Parse<ConsoloniaTheme>(settings.ConsoloniaTheme);
            this.UIThemeVariant = settings.LightVariant ? ThemeVariant.Light : ThemeVariant.Dark;
            this.ShowTabs = settings.ShowTabs;
            this.ShowSpaces = settings.ShowSpaces;
            this.DefaultExtension = settings.DefaultExtension;
            this.SyntaxTheme = Enum.Parse<ThemeName>(settings.SyntaxTheme ?? ThemeName.VisualStudioDark.ToString());
        }

        public static AppViewModel LoadSettings()
        {
            var appViewModel = new AppViewModel();
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    var settings = JsonSerializer.Deserialize<Settings>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    appViewModel.SetSettings(settings ?? new Settings());
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (JsonException)
            {
            }
            return appViewModel;
        }

        public async void SaveSettings()
        {
            try
            {
                string dir = Path.GetDirectoryName(SettingsFilePath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string json = JsonSerializer.Serialize(GetSettings(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (IOException err)
            {
                await MessageBox.ShowDialog("Settings Error", $"Failed to save settings file {SettingsFilePath}. {err.Message}");
            }
            catch (UnauthorizedAccessException err2)
            {
                await MessageBox.ShowDialog("Settings Error", err2.Message);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception err)
            {
                await MessageBox.ShowDialog("Settings Error", $"Failed to save settings: {err.Message}");
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}