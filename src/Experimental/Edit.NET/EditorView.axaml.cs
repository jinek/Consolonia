using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Media;
using AvaloniaEdit.TextMate;
using System.Linq;
using Avalonia.Styling;
using Consolonia.Themes;
using Consolonia.Controls;

namespace Edit.NET
{

    public partial class EditorView : UserControl
    {

        public EditorView()
        {
            this.DataContext = new EditorViewModel();

            InitializeComponent();

            ViewModel.Editor = Editor;
            ViewModel.TextMateInstallation = new TextMate.Installation(ViewModel.Editor, ViewModel.RegistryOptions);

            // Wire up editor events for status updates
            Editor.AttachedToVisualTree += (_, __) => { UpdateStatus(); };
            Editor.TextChanged += (_, __) =>
            {
                ViewModel.Modified = true;
                UpdateStatus();
            };
            Editor.TextArea.Caret.PositionChanged += (_, __) => UpdateStatus();

            // Install TextMate syntax highlighting similar to Consolonia.Editor
            ViewModel.TextMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;

            // Default to C# highlighting
            ViewModel.Syntax = EditorSyntax.PlainText;

            Loaded += OnLoaded;
        }

        public EditorViewModel ViewModel
        {
            get => (EditorViewModel)DataContext!;
            set => DataContext = value;
        }

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



        private void OnThemeVariantLightMenuClick(object sender, RoutedEventArgs e)
        {
            var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime.MainWindow;
            mainWindow.RequestedThemeVariant = ThemeVariant.Light;
            UpdateThemeMenuItems();
        }

        private void OnThemeVariantDarkMenuClick(object sender, RoutedEventArgs e)
        {
            var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime.MainWindow;
            mainWindow.RequestedThemeVariant = ThemeVariant.Dark;
            UpdateThemeMenuItems();
        }

        private void OnThemeMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem { Tag: string themeName } ||
                !Enum.TryParse(themeName, out ThemesList selectedTheme))
                return;

            var viewModel = (EditorViewModel)DataContext!;
            if (viewModel.Modified)
            {
                MessageBox.ShowDialog(themeName, "You have unsaved changes. You need to save your file before you change themes.");
                return;
            }

            // NOTE: this assumes first style object is the old theme!
            Application.Current.Styles[0] = selectedTheme switch
            {
                ThemesList.Modern => new ModernTheme(),
                ThemesList.ModernContrast => new ModernContrastTheme(),
                ThemesList.TurboVision => new TurboVisionTheme(),
                ThemesList.TurboVisionCompatible => new TurboVisionCompatibleTheme(),
                ThemesList.TurboVisionGray => new TurboVisionGrayTheme(),
                ThemesList.TurboVisionElegant => new TurboVisionElegantTheme(),
                _ => throw new InvalidDataException("Unknown theme name")
            };
            ViewModel.CurrentTheme = selectedTheme.ToString();
            var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime.MainWindow;
            var newView = new EditorView() { DataContext = viewModel };
            mainWindow.Content = newView;
            
            ViewModel.New();
        }

        private void OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            UpdateThemeMenuItems();
        }

        private void UpdateThemeMenuItems()
        {
            ViewModel.CurrentTheme = Application.Current.Styles[0].GetType().Name[..^5];

            ThemeDarkMenuItem.IsChecked = ActualThemeVariant == ThemeVariant.Dark;
            ThemeLightMenuItem.IsChecked = ActualThemeVariant == ThemeVariant.Light;
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
            ApplyBrushAction(e, "editor.foreground", brush => Editor.Foreground = brush);

            // Selection brush
            if (!ApplyBrushAction(e, "editor.selectionBackground", brush => Editor.TextArea.SelectionBrush = brush))
            {
                ApplyBrushAction(e, "editor.selectionHighlightBackground", brush => Editor.TextArea.SelectionBrush = brush);
            }

            // Current line highlight
            if (!ApplyBrushAction(e, "editor.lineHighlightBackground", brush => Editor.TextArea.TextView.CurrentLineBackground = brush))
            {
                Editor.TextArea.TextView.SetDefaultHighlightLineColors();
            }

            // Line numbers
            if (!ApplyBrushAction(e, "editorLineNumber.foreground", brush => Editor.LineNumbersForeground = brush))
            {
                Editor.LineNumbersForeground = Editor.Foreground;
            }
        }

        private void ApplyThemeColorsToWindow(TextMate.Installation e)
        {
            // Status bar/background panel
            if (this.FindControl<Border>("BottomPanel") is { } bottom)
            {
                ApplyBrushAction(e, "statusBar.background", brush => bottom.Background = brush);
            }

            // Apply editor theme colors to the window for better contrast
            ApplyBrushAction(e, "editor.background", brush => Background = brush);
            ApplyBrushAction(e, "editor.foreground", brush => Foreground = brush);
        }

        private bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
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
    }
}