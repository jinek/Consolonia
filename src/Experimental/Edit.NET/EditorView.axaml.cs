using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using System.Linq;
using Avalonia.Styling;
using Consolonia.Themes;
using Consolonia.Controls;
using Avalonia.Threading;

namespace Edit.NET
{

    public partial class EditorView : UserControl
    {
        private RegistryOptions? _registryOptions;
        private TextMate.Installation? _textMateInstallation;

        public EditorView()
        {
            this.DataContext = new EditorViewModel();

            InitializeComponent();

            ViewModel.Editor = Editor;
            // Wire up editor events for status updates
            Editor.AttachedToVisualTree += (_, __) => { UpdateStatus(); };
            Editor.TextChanged += (_, __) =>
            {
                ViewModel.Modified = true;
                UpdateStatus();
            };
            Editor.TextArea.Caret.PositionChanged += (_, __) => UpdateStatus();

            // Install TextMate syntax highlighting similar to Consolonia.Editor
            _registryOptions = new RegistryOptions(ThemeName.HighContrastDark);
            _textMateInstallation = Editor.InstallTextMate(_registryOptions);
            _textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;

            // Default to C# highlighting
            ApplySyntax(EditorSyntax.CSharp);

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


        private void SyntaxPlain_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Syntax = EditorSyntax.PlainText;
            ApplySyntax(ViewModel.Syntax);
            SetChecked(sender);
        }

        private void SyntaxCSharp_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Syntax = EditorSyntax.CSharp;
            ApplySyntax(ViewModel.Syntax);
            SetChecked(sender);
        }

        private void SetChecked(object sender)
        {
            var menuItem1 = (MenuItem)sender;
            menuItem1.IsChecked = true;

            foreach (MenuItem item in ((MenuItem)menuItem1.Parent!).Items.Cast<MenuItem>()
                     .Where(item => item != menuItem1)) item.IsChecked = false;
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

            var lifetime = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime.MainWindow;
            var newView = new EditorView() { DataContext = viewModel };
            mainWindow.Content = newView;
        }

        private void OnLoaded(object? sender, RoutedEventArgs routedEventArgs)
        {
            UpdateThemeMenuItems();
        }

        private void UpdateThemeMenuItems()
        {
            string themeName = Application.Current.Styles[0].GetType().Name[..^5];
            ThemeModernMenuItem.IsChecked = themeName == nameof(ThemesList.Modern);
            ThemeModernContrastMenuItem.IsChecked = themeName == nameof(ThemesList.ModernContrast);
            ThemeTurboVisionMenuItem.IsChecked = themeName == nameof(ThemesList.TurboVision);
            ThemeTurboVisionCompatibleMenuItem.IsChecked = themeName == nameof(ThemesList.TurboVisionCompatible);
            ThemeTurboVisionGrayMenuItem.IsChecked = themeName == nameof(ThemesList.TurboVisionGray);
            ThemeTurboVisionElegantMenuItem.IsChecked = themeName == nameof(ThemesList.TurboVisionElegant);

            ThemeDarkMenuItem.IsChecked = ActualThemeVariant == ThemeVariant.Dark;
            ThemeLightMenuItem.IsChecked = ActualThemeVariant == ThemeVariant.Light;
        }

        private void SyntaxXml_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Syntax = EditorSyntax.Xml;
            ApplySyntax(ViewModel.Syntax);
            SetChecked(sender);
        }

        private void SyntaxHtml_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Syntax = EditorSyntax.Html;
            ApplySyntax(ViewModel.Syntax);
            SetChecked(sender);
        }

        private void SyntaxJavaScript_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Syntax = EditorSyntax.JavaScript;
            ApplySyntax(ViewModel.Syntax);
            SetChecked(sender);
        }

        private void SetSyntaxByExtension(string ext)
        {
            if (_registryOptions == null || _textMateInstallation == null)
                return;
            var lang = _registryOptions.GetLanguageByExtension(ext);
            if (lang == null)
            {
                _textMateInstallation.SetGrammar(null);
                return;
            }
            var scope = _registryOptions.GetScopeByLanguageId(lang.Id);
            _textMateInstallation.SetGrammar(scope);
        }

        private void ApplySyntax(EditorSyntax syntax)
        {
            if (_registryOptions == null || _textMateInstallation == null)
                return;

            switch (syntax)
            {
                case EditorSyntax.PlainText:
                    _textMateInstallation.SetGrammar(null);
                    break;
                case EditorSyntax.CSharp:
                    SetSyntaxByExtension(".cs");
                    break;
                case EditorSyntax.Xml:
                    SetSyntaxByExtension(".xml");
                    break;
                case EditorSyntax.Html:
                    SetSyntaxByExtension(".html");
                    break;
                case EditorSyntax.JavaScript:
                    SetSyntaxByExtension(".js");
                    break;
                case EditorSyntax.Json:
                    SetSyntaxByExtension(".json");
                    break;
                default:
                    _textMateInstallation.SetGrammar(null);
                    break;
            }
        }

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