using System;
using System.IO;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Consolonia;
using Consolonia.Themes;
using EditNET.ViewModels;
using EditNET.Views;
using ReactiveUI;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace EditNET
{
    public class App : Application
    {
        private WindowNotificationManager? _notificationManager;

        private MainWindow MainWindow =>
            (MainWindow)((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime!).MainWindow!;

        private AppViewModel ViewModel => (AppViewModel)DataContext!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            DataContext = new AppViewModel();
            ViewModel.SetThemeInteraction.RegisterHandler(SetThemeHandler);
            ViewModel.ShowNotificationInteraction.RegisterHandler(ShowNotificationHandler);
        }

        private void ShowNotificationHandler(IInteractionContext<Notification, Unit> context)
        {
            _notificationManager!.Show(context.Input);
            context.SetOutput(Unit.Default);
        }

        private void SetThemeHandler(IInteractionContext<(ConsoloniaTheme, bool), Unit> context)
        {
            context.SetOutput(Unit.Default);
            MainWindow.RequestedThemeVariant = context.Input.Item2 ? ThemeVariant.Light : ThemeVariant.Dark;
            if (!LoadUITheme(context.Input.Item1))
            {
                ShowThemeIncompatible();
                return;
            }

            MainWindow.Content = new EditorView { DataContext = ViewModel.EditorViewModel };
        }

        public override async void OnFrameworkInitializationCompleted()
        {
            bool themeLoaded = false;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                themeLoaded = LoadUITheme(ViewModel.EditorViewModel.Settings.ConsoloniaTheme);
                desktopLifetime.MainWindow = new MainWindow
                {
                    DataContext = ViewModel,
                    RequestedThemeVariant = ViewModel.EditorViewModel.Settings.LightVariant
                        ? ThemeVariant.Light
                        : ThemeVariant.Default
                };

                if (desktopLifetime.Args is { Length: > 0 })
                {
                    var filePath = desktopLifetime.Args[0];
                    if (!Path.IsPathFullyQualified(desktopLifetime.Args[0]))
                        filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, desktopLifetime.Args[0]));
                    await ViewModel.EditorViewModel.OpenFile(filePath);
                }

                _notificationManager = new WindowNotificationManager(desktopLifetime.MainWindow!);
            }

            base.OnFrameworkInitializationCompleted();

            if (ViewModel.InitialLoadSettingsException != null)
                ShowNotification("Settings Error",
                    "Failed to load settings: " + ViewModel.InitialLoadSettingsException.Message,
                    NotificationType.Error);

            if (!themeLoaded) ShowThemeIncompatible();
        }

        private void ShowThemeIncompatible()
        {
            ShowNotification("Theme Error",
                $"Modern themes are not supported in this environment. Switching back to {nameof(ConsoloniaTheme.TurboVisionCompatible)} and {Enum.GetName(EditorView.DefaultEditorTheme)}",
                NotificationType.Error);
        }

        private void ShowNotification(string title, string message, NotificationType notificationType)
        {
            _notificationManager!.Show(new Notification { Type = notificationType, Message = message, Title = title });
        }

        private bool LoadUITheme(ConsoloniaTheme theme)
        {
            if (!((ConsoloniaLifetime)ApplicationLifetime!).IsRgbColorMode() &&
                theme is ConsoloniaTheme.Modern or ConsoloniaTheme.ModernContrast)
                return false;

            Styles[0] = theme switch
            {
                ConsoloniaTheme.Modern => new ModernTheme(),
                ConsoloniaTheme.ModernContrast => new ModernContrastTheme(),
                ConsoloniaTheme.TurboVision => new TurboVisionTheme(),
                ConsoloniaTheme.TurboVisionCompatible => new TurboVisionCompatibleTheme(),
                ConsoloniaTheme.TurboVisionGray => new TurboVisionGrayTheme(),
                ConsoloniaTheme.TurboVisionElegant => new TurboVisionElegantTheme(),
                _ => throw new InvalidDataException("Unknown theme name")
            };

            return true;
        }
    }
}