using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using Iciclecreek.Avalonia.WindowManager;
using TextMateSharp.Grammars;

namespace Consolonia.Gallery.View
{
    public partial class CodeDialog : ManagedWindow
    {
        private readonly RegistryOptions _registryOptions;
        private readonly TextEditor _textEditor;
        private readonly TextMate.Installation _textMateInstallation;

        public CodeDialog(string file, string text)
        {
            InitializeComponent();

            Title = file;

            _textEditor = this.FindControl<TextEditor>("Editor")!;
            _textEditor.Document = new TextDocument(text);
            ThemeName theme = Application.Current.RequestedThemeVariant == ThemeVariant.Dark
                ? ThemeName.VisualStudioDark
                : ThemeName.VisualStudioLight;
            _registryOptions = new RegistryOptions(theme);
            _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);
            Language language = _registryOptions.GetLanguageByExtension(Path.GetExtension(file));
            string scopeName = _registryOptions.GetScopeByLanguageId(language.Id);
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(language.Id));
            TextMateInstallationOnAppliedTheme(this, _textMateInstallation);
        }

        private void TextMateInstallationOnAppliedTheme(object sender, TextMate.Installation e)
        {
            ApplyBrushAction(e, "editor.background", brush => _textEditor.Background = brush);
            ApplyBrushAction(e, "editor.foreground", brush => _textEditor.TextArea.Foreground = brush);

            if (!ApplyBrushAction(e, "editor.selectionBackground",
                    brush => _textEditor.TextArea.SelectionBrush = brush))
                if (!ApplyBrushAction(e, "editor.selectionHighlightBackground",
                        brush => _textEditor.TextArea.SelectionBrush = brush))
                    if (Application.Current!.TryGetResource("TextAreaSelectionBrush", out object resourceObject))
                        if (resourceObject is IBrush brush)
                            _textEditor.TextArea.SelectionBrush = brush;

            if (!ApplyBrushAction(e, "editor.lineHighlightBackground",
                    brush => _textEditor.TextArea.TextView.CurrentLineBackground = brush))
                _textEditor.TextArea.TextView.SetDefaultHighlightLineColors();

            //Todo: looks like the margin doesn't have a active line highlight, would be a nice addition
            if (!ApplyBrushAction(e, "editorLineNumber.foreground",
                    brush => _textEditor.LineNumbersForeground = brush))
                _textEditor.LineNumbersForeground = _textEditor.TextArea.Foreground;
            _textEditor.TextArea.TextView.CurrentLineBorder = new Pen(Brushes.Transparent, 0);
        }

        private bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson,
            Action<IBrush> applyColorAction)
        {
            if (!e.TryGetThemeColor(colorKeyNameFromJson, out string colorString))
                return false;

            if (!Color.TryParse(colorString, out Color color))
                return false;

            var colorBrush = new SolidColorBrush(color);
            applyColorAction(colorBrush);
            return true;
        }
    }
}