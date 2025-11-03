using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit.TextMate;
using EditNET.DataModels;
using EditNET.ViewModels;
using TextMateSharp.Grammars;

namespace EditNET.Views
{

    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();
            Editor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy(Editor.Options);
            Editor.TextArea.RightClickMovesCaret = true;

            // Wire up editor events for status updates
            Editor.AttachedToVisualTree += (_, __) => { UpdateStatus(); };
            Editor.TextChanged += (_, __) =>
            {
                ViewModel.Modified = true;
                UpdateStatus();
            };
            Editor.TextArea.Caret.PositionChanged += (_, __) => UpdateStatus();

            var registryOptions = new RegistryOptions(ThemeName.VisualStudioDark);
            var textMateInstallation = TextMate.InstallTextMate(Editor, registryOptions);
            // Install TextMate syntax highlighting similar to Consolonia.Editor
            textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;
            ApplyThemeColorsToEditor(textMateInstallation);

            this.Editor.Options.ShowSpaces = App.ViewModel.ShowSpaces;
            this.Editor.Options.ShowTabs = App.ViewModel.ShowTabs;

            // Default to plaintext
            this.DataContext = new EditorViewModel()
            {
                Editor = this.Editor,
                TextMateInstallation = textMateInstallation,
                RegistryOptions = registryOptions,
                Syntax = registryOptions.GetLanguageByExtension(".txt")
            };

            Loaded += OnLoaded;
        }

        public EditorViewModel ViewModel
        {
            get => (EditorViewModel)DataContext!;
            set => DataContext = value;
        }

        public static IClassicDesktopStyleApplicationLifetime Lifetime
            => (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;

        public static Window MainWindow
            => Lifetime!.MainWindow!;

        private void UpdateStatus()
        {
            // Position
            var line = Editor.TextArea.Caret.Line;
            var column = Editor.TextArea.Caret.Column;
            PositionText.Text = $"Ln {line}, Col {column}";
            // Length
            var length = Editor.Document?.TextLength ?? (Editor.Text?.Length ?? 0);
            LengthText.Text = $"Len {length}";
        }

        private void OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            Editor.TextArea.Focus();
        }

        private void TextMateInstallationOnAppliedTheme(object? sender, TextMate.Installation e)
        {
            ApplyThemeColorsToEditor(e);
        }

        private void ApplyThemeColorsToEditor(TextMate.Installation e)
        {
            ApplyBrushAction(e, "editor.background", brush => Editor.Background = brush);
            ApplyBrushAction(e, "editor.foreground", brush => Editor.TextArea.Foreground = brush);

            // Selection brush
            if (!ApplyBrushAction(e, "editor.selectionBackground", brush => Editor.TextArea.SelectionBrush = brush))
            {
                ApplyBrushAction(e, "editor.selectionHighlightBackground", brush => Editor.TextArea.SelectionBrush = brush);
            }

            // Current line highlight
            if (!ApplyBrushAction(e, "editor.lineHighlightBackground", brush => Editor.TextArea.TextView.CurrentLineBackground = brush))
            {
                // Editor.TextArea.TextView.SetDefaultHighlightLineColors();
            }

            // Line numbers
            if (!ApplyBrushAction(e, "editorLineNumber.foreground", brush => Editor.LineNumbersForeground = brush))
            {
                Editor.LineNumbersForeground = Editor.TextArea.Foreground;
            }
        }

        private static bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
        {
            if (!e.TryGetThemeColor(colorKeyNameFromJson, out var colorString))
                return false;
            if (!Color.TryParse(colorString, out var color))
                return false;

            var colorBrush = new SolidColorBrush(color);
            applyColorAction(colorBrush);
            return true;
        }

        private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog(this);
        }

        private async void OnShowSettings(object? sender, RoutedEventArgs e)
        {
            var dlg = new EditSettingsDialog(App.ViewModel.GetSettings());
            var newSettings = await dlg.ShowDialog<Settings>(this);
            if (newSettings != null)
            {
                App.ViewModel.SetSettings(newSettings);
                App.ViewModel.SaveSettings();
                this.Editor.Options.ShowSpaces = newSettings.ShowSpaces;
                this.Editor.Options.ShowTabs = newSettings.ShowTabs;
                this.Editor.TextArea.Focus();
            }
        }
    }
}