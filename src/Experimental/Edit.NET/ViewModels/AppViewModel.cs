using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Controls;
using EditNET.DataModels;
using ReactiveUI;
using TextMateSharp.Grammars;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace EditNET.ViewModels
{
    public partial class AppViewModel : ObservableObject
    {
        public const string EditNetTitle = "Edit.NET";

        private static readonly string SettingsFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                EditNetTitle,
                "settings.json");

        public EditorViewModel EditorViewModel { get; private set; }
        public Interaction<(ConsoloniaTheme, bool), Unit> SetThemeInteraction { get; } = new();
        public Interaction<Notification, Unit> ShowNotificationInteraction { get; } = new();
        private ConsoloniaTheme _consoloniaTheme;
        private bool _consoloniaThemeLight;
        public Exception? InitialLoadSettingsException { get; }

        public AppViewModel()
        {
            Settings? settings = null;
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    settings = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.Settings);
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or JsonException)
            {
                InitialLoadSettingsException = exception;
            }

            settings ??= new Settings();
            _consoloniaTheme = settings.ConsoloniaTheme;
            _consoloniaThemeLight = settings.LightVariant;
            EditorViewModel = new EditorViewModel(settings);
            EditorViewModel.WhenAnyValue(model => model.Settings).Skip(1).Subscribe(OnSettingsUpdated);
        }

        private void OnSettingsUpdated(Settings settings)
        {
            SaveSettings(settings);
            if (_consoloniaTheme != settings.ConsoloniaTheme || _consoloniaThemeLight != settings.LightVariant)
            {
                SetThemeInteraction.Handle((settings.ConsoloniaTheme, settings.LightVariant)).Wait();
                _consoloniaTheme = settings.ConsoloniaTheme;
                _consoloniaThemeLight = settings.LightVariant;
            }
        }

        private async void SaveSettings(Settings settings)
        {
            try
            {
                string dir = Path.GetDirectoryName(SettingsFilePath)!;
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string json = JsonSerializer.Serialize(settings, SettingsJsonContext.Default.Settings);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                await ShowNotificationInteraction.Handle(new Notification("Settings Error",
                    "Failed to save settings file: " + exception.Message, NotificationType.Error));
            }
        }
    }
}