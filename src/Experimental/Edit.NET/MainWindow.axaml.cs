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

namespace Edit.NET
{
    public partial class MainWindow : Window
    {
        private Avalonia.Platform.Storage.IStorageFile? _currentFile;
        private string? _currentFileName;
        private bool _isModified;
        private RegistryOptions? _registryOptions;
        private TextMate.Installation? _textMateInstallation;

        public MainWindow()
        {
            InitializeComponent();
            // Wire up editor events for status updates
            Editor.TextChanged += (_, __) => { _isModified = true; UpdateStatus(); };
            Editor.AttachedToVisualTree += (_, __) => UpdateStatus();
            Editor.TextArea.Caret.PositionChanged += (_, __) => UpdateStatus();

            // Install TextMate syntax highlighting similar to Consolonia.Editor
            _registryOptions = new RegistryOptions(ThemeName.HighContrastDark);
            _textMateInstallation = Editor.InstallTextMate(_registryOptions);
            _textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;

            // Default to C# highlighting
            var csharp = _registryOptions.GetLanguageByExtension(".cs");
            var scope = _registryOptions.GetScopeByLanguageId(csharp.Id);
            _textMateInstallation.SetGrammar(scope);
        }

        private void UpdateStatus()
        {
            // File name
            FileNameText.Text = string.IsNullOrEmpty(_currentFileName) ? "[No Name]" : _currentFileName;
            // Position
            var line = Editor.TextArea.Caret.Line;
            var column = Editor.TextArea.Caret.Column;
            PositionText.Text = $"Ln {line}, Col {column}";
            // Length
            var length = Editor.Document?.TextLength ?? (Editor.Text?.Length ?? 0);
            LengthText.Text = $"Len {length}";
            // Modified flag
            ModifiedText.Text = _isModified ? "Modified" : "Saved";
        }

        private void ResetModified()
        {
            _isModified = false;
            UpdateStatus();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            var lifetime = Application.Current!.ApplicationLifetime as IControlledApplicationLifetime;
            lifetime!.Shutdown();
        }

        private void OnNewFile_OnClick(object? sender, RoutedEventArgs e)
        {
            Editor.Text = string.Empty;
            _currentFile = null;
            _currentFileName = null;
            ResetModified();
        }

        private void Exit_OnClick(object? sender, RoutedEventArgs e)
        {
            var lifetime = (IControlledApplicationLifetime)Application.Current!.ApplicationLifetime!;
            lifetime.Shutdown();
        }

        private void Cut_OnClick(object? sender, RoutedEventArgs e)
        {
            Editor.Cut();
        }

        private async void Open_OnClick(object? sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Open File"
            });
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                try
                {
                    await using var stream = await file.OpenReadAsync();
                    using var reader = new StreamReader(stream);
                    var text = await reader.ReadToEndAsync();
                    Editor.Text = text;
                    _currentFile = file;
                    _currentFileName = file.Name;
                    ResetModified();
                }
                catch (IOException ex)
                {
                    await ShowMessageAsync("Open Error", ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    await ShowMessageAsync("Open Error", ex.Message);
                }
            }
        }

        private async void Save_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_currentFile == null)
            {
                await SaveAsInternalAsync();
            }
            else
            {
                await SaveToFileAsync(_currentFile);
            }
        }

        private async void SaveAs_OnClick(object? sender, RoutedEventArgs e)
        {
            await SaveAsInternalAsync();
        }

        private async Task SaveAsInternalAsync()
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save As",
                SuggestedFileName = _currentFileName ?? "Untitled.txt"
            });
            if (file != null)
            {
                await SaveToFileAsync(file);
            }
        }

        private async Task SaveToFileAsync(IStorageFile file)
        {
            try
            {
                await using var stream = await file.OpenWriteAsync();
                stream.SetLength(0);
                await using var writer = new StreamWriter(stream);
                await writer.WriteAsync(Editor.Text ?? string.Empty);
                _currentFile = file;
                _currentFileName = file.Name;
                ResetModified();
            }
            catch (IOException ex)
            {
                await ShowMessageAsync("Save Error", ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await ShowMessageAsync("Save Error", ex.Message);
            }
        }

        private async Task ShowMessageAsync(string title, string message)
        {
            var dlg = new Window
            {
                Title = title,
                Width = 400,
                Height = 200,
                Content = new StackPanel
                {
                    Margin = new Thickness(12),
                    Children =
                    {
                        new TextBlock { Text = message, TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                        new Button { Content = "OK", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, Margin = new Thickness(0,12,0,0) }
                    }
                }
            };
            if (dlg.Content is StackPanel sp && sp.Children.Count > 1 && sp.Children[1] is Button btn)
            {
                btn.Click += (_, __) => dlg.Close();
            }
            await dlg.ShowDialog(this);
        }

        private void SyntaxPlain_OnClick(object? sender, RoutedEventArgs e)
        {
            _textMateInstallation?.SetGrammar(null);
        }

        private void SyntaxCSharp_OnClick(object? sender, RoutedEventArgs e)
        {
            SetSyntaxByExtension(".cs");
        }

        private void SyntaxXml_OnClick(object? sender, RoutedEventArgs e)
        {
            SetSyntaxByExtension(".xml");
        }

        private void SyntaxHtml_OnClick(object? sender, RoutedEventArgs e)
        {
            SetSyntaxByExtension(".html");
        }

        private void SyntaxJavaScript_OnClick(object? sender, RoutedEventArgs e)
        {
            SetSyntaxByExtension(".js");
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
    }
}