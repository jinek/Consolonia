using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit.TextMate;
using Consolonia.Themes;
using Edit.NET.DataModels;
using TextMateSharp.Grammars;

namespace Edit.NET
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
            var textMateInstallation = Editor.InstallTextMate(registryOptions);
            // Install TextMate syntax highlighting similar to Consolonia.Editor
            textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;
            ApplyThemeColorsToEditor(textMateInstallation);

            // Default to plaintext
            this.DataContext = new EditorViewModel()
            {
                Editor = this.Editor,
                TextMateInstallation = textMateInstallation,
                RegistryOptions = registryOptions,
                Syntax = EditorSyntax.PlainText
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
            UpdateThemeMenuItems();
            Editor.TextArea.Focus();
        }

        private void UpdateThemeMenuItems()
        {
            try
            {
                if (Application.Current != null && Application.Current.Styles.Count > 0)
                    ViewModel.CurrentTheme = Application.Current.Styles[0].GetType().Name[..^5];
            }
            catch (ArgumentOutOfRangeException)
            {
                // ignore
            }
            catch (NullReferenceException)
            {
                // ignore
            }
        }

        private void SyntaxMarkdown_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.Markdown;

        private void SyntaxPlain_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.PlainText;

        private void SyntaxCSharp_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.CSharp;

        private void SyntaxXml_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.Xml;

        private void SyntaxHtml_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.Html;

        private void SyntaxJavaScript_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.JavaScript;

        private void SyntaxJson_OnClick(object sender, RoutedEventArgs e)
            => ViewModel.Syntax = EditorSyntax.Json;

        private void TextMateInstallationOnAppliedTheme(object? sender, TextMate.Installation e)
        {
            ApplyThemeColorsToEditor(e);
            ApplyThemeColorsToWindow(e);
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

        private void ApplyThemeColorsToWindow(TextMate.Installation e)
        {
            // Status bar/background panel
            //if (this.FindControl<Border>("BottomPanel") is { } bottom)
            //{
            //    ApplyBrushAction(e, "statusBar.background", brush => bottom.Background = brush);
            //}

            // Apply editor theme colors to the window for better contrast
            //ApplyBrushAction(e, "editor.background", brush => Background = brush);
            //ApplyBrushAction(e, "editor.foreground", brush => Foreground = brush);
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
            var dlg = new EditSettingsDialog(((App)Application.Current).ViewModel.GetSettings());
            var newSettings = await dlg.ShowDialog<Settings>(this);
            if (newSettings != null)
            {
                ((App)Application.Current).ViewModel.SetSettings(newSettings);
                ((App)Application.Current).ViewModel.SaveSettings();
                UpdateThemeMenuItems();
            }
        }
    }
}