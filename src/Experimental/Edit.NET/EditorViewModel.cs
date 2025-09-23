using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consolonia.Controls;
using ReactiveUI;
using TextMateSharp.Grammars;

namespace Edit.NET
{
    public enum EditorSyntax
    {
        PlainText,
        Markdown,
        CSharp,
        Xml,
        Html,
        JavaScript,
        Json,
        Unknown
    }

    public partial class EditorViewModel : ObservableObject
    {
        public EditorViewModel()
        {
            FilePath = Path.Combine(CurrentFolder, "Untitled.txt");
            RegistryOptions = new RegistryOptions(ThemeName.LightPlus);
            // call ApplySyntax when Syntax changes
            this.WhenAnyValue(x => x.Syntax).Subscribe(ApplySyntax);
        }

        public TextEditor Editor { get; set; }
        public RegistryOptions? RegistryOptions { get; set; }
        public TextMate.Installation? TextMateInstallation { get; set; }

        [NotifyPropertyChangedFor(nameof(FileName))]
        [NotifyPropertyChangedFor(nameof(FileNameOnly))]
        [NotifyPropertyChangedFor(nameof(Extension))]
        [ObservableProperty]
        private string? _filePath;

        public string? FileName => Path.GetFileName(FilePath);

        public string? FileNameOnly => Path.GetFileNameWithoutExtension(FilePath);

        public string? Extension => Path.GetExtension(FilePath);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsSyntaxPlainText))]
        [NotifyPropertyChangedFor(nameof(IsSyntaxMarkdown))]
        [NotifyPropertyChangedFor(nameof(IsSyntaxCSharp))]
        [NotifyPropertyChangedFor(nameof(IsSyntaxXml))]
        [NotifyPropertyChangedFor(nameof(IsSyntaxHtml))]
        [NotifyPropertyChangedFor(nameof(IsSyntaxJavascript))]
        [NotifyPropertyChangedFor(nameof(IsSyntaxJson))]
        private EditorSyntax _syntax;

        public bool IsSyntaxPlainText => Syntax == EditorSyntax.PlainText;
        public bool IsSyntaxMarkdown => Syntax == EditorSyntax.Markdown;
        public bool IsSyntaxCSharp => Syntax == EditorSyntax.CSharp;
        public bool IsSyntaxXml => Syntax == EditorSyntax.Xml;
        public bool IsSyntaxHtml => Syntax == EditorSyntax.Html;
        public bool IsSyntaxJavascript => Syntax == EditorSyntax.JavaScript;
        public bool IsSyntaxJson => Syntax == EditorSyntax.Json;


        [ObservableProperty]
        private bool _modified;

        [ObservableProperty]
        private string _currentFolder = Environment.CurrentDirectory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsThemeModern))]
        [NotifyPropertyChangedFor(nameof(IsThemeModernContrast))]
        [NotifyPropertyChangedFor(nameof(IsThemeTurboVision))]
        [NotifyPropertyChangedFor(nameof(IsThemeTurboVisionCompatible))]
        [NotifyPropertyChangedFor(nameof(IsThemeTurboVisionGray))]
        [NotifyPropertyChangedFor(nameof(IsThemeTurboVisionElegant))]
        private string _currentTheme;

        public bool IsThemeModern => CurrentTheme == "Modern";
        public bool IsThemeModernContrast => CurrentTheme == "ModernContrast";
        public bool IsThemeTurboVision => CurrentTheme == "TurboVision";
        public bool IsThemeTurboVisionCompatible => CurrentTheme == "TurboVisionCompatible";
        public bool IsThemeTurboVisionGray => CurrentTheme == "TurboVisionGray";
        public bool IsThemeTurboVisionElegant => CurrentTheme == "TurboVisionElegant";
        public bool IsThemeLight => CurrentTheme == "Light";
        public bool IsThemeDark => CurrentTheme == "Dark";

        public async Task OpenFile(string? path)
        {
            FilePath = path;
            Syntax = GetSyntaxFromExtension(Extension);
            Editor.Text = File.Exists(path) ? await File.ReadAllTextAsync(path) : string.Empty;
            Modified = false;
            CurrentFolder = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory;
        }

        public async Task SaveFileAsync()
        {
            try
            {
                await File.WriteAllTextAsync(FilePath, Editor.Text);
                Modified = false;
                CurrentFolder = Path.GetDirectoryName(FilePath) ?? Environment.CurrentDirectory;
            }
            catch (IOException ex)
            {
                await MessageBox.ShowDialog("Save Error", ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await MessageBox.ShowDialog("Save Error", ex.Message);
            }
        }

        public async Task SaveAsToPathAsync(string fullPath)
        {
            FilePath = fullPath;
            await SaveFileAsync();
        }

        [RelayCommand(CanExecute = nameof(CanCut))]
        public void Cut() => Editor.Cut();
        public bool CanCut() => Editor.CanCut;


        [RelayCommand(CanExecute = nameof(CanCopy))]
        public void Copy() => Editor.Copy();
        public bool CanCopy() => Editor.CanCopy;


        [RelayCommand(CanExecute = nameof(CanPaste))]
        public void Paste() => Editor.Paste();
        public bool CanPaste() => Editor.CanPaste;

        [RelayCommand(CanExecute = nameof(CanUndo))]
        public void Undo() => Editor.Undo();
        public bool CanUndo() => Editor.CanUndo;

        [RelayCommand(CanExecute = nameof(CanRedo))]
        public void Redo() => Editor.Redo();
        public bool CanRedo() => Editor.CanRedo;

        [RelayCommand]
        public void SelectAll() => Editor.SelectAll();


        [RelayCommand]
        public async Task New()
        {
            if (Modified)
            {
                var result = await MessageBox.ShowDialog("Unsaved Changes", "You have unsaved changes. Do you want to discard them?", MessageBoxStyle.YesNoCancel);
                if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
                    return;
            }

            FilePath = Path.Combine(CurrentFolder, $"Untitled.txt");
            Editor.Text = string.Empty;
            Syntax = GetSyntaxFromExtension(Path.GetExtension(FilePath));
            Modified = false;
            Editor.TextArea.Focus();
        }

        [RelayCommand]
        public async Task Open()
        {
            if (Modified)
            {
                var result = await MessageBox.ShowDialog("Unsaved Changes", "You have unsaved changes. Do you want to discard them?", MessageBoxStyle.YesNoCancel);
                if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
                    return;
            }

            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return;
            var mainWindow = desktop.MainWindow;
            var files = await mainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedStartLocation = await mainWindow.StorageProvider.TryGetFolderFromPathAsync(CurrentFolder),
                Title = "Open File"
            });
            if (files != null && files.Count > 0)
            {
                var file = files[0];
                try
                {
                    await OpenFile(Path.GetFullPath(file.Path.AbsolutePath));
                }
                catch (IOException ex)
                {
                    await MessageBox.ShowDialog("Open Error", ex.Message);
                }
                catch (UnauthorizedAccessException ex)
                {
                    await MessageBox.ShowDialog("Open Error", ex.Message);
                }
            }
            Editor.TextArea.Focus();
        }

        [RelayCommand(CanExecute =nameof(CanSave))]
        private async Task Save()
        {
            if (string.IsNullOrEmpty(FilePath))
            {
                if (SaveAsCommand is IAsyncRelayCommand asyncCmd)
                    await asyncCmd.ExecuteAsync(null);
                else
                    SaveAsCommand?.Execute(null);
                return;
            }
            await SaveFileAsync();
        }
        public bool CanSave() => Modified;

        [RelayCommand]
        private async Task SaveAs()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return;
            var mainWindow = desktop.MainWindow;
            var file = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save As",
                SuggestedStartLocation = await mainWindow.StorageProvider.TryGetFolderFromPathAsync(CurrentFolder),
                SuggestedFileName = FileName ?? "Untitled.txt"
            });
            if (file != null)
            {
                FilePath = Path.GetFullPath(file.Path.AbsolutePath);
                await SaveFileAsync();
            }
            Editor.TextArea.Focus();
        }

        [RelayCommand]
        public void Exit()
        {
            if (Application.Current?.ApplicationLifetime is IControlledApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        }

        public static EditorSyntax GetSyntaxFromExtension(string? extension)
        {
            if (string.IsNullOrWhiteSpace(extension))
                return EditorSyntax.PlainText;

            switch (extension.ToLowerInvariant())
            {
                case ".md":
                    return EditorSyntax.Markdown;
                case ".txt":
                    return EditorSyntax.PlainText;
                case ".cs":
                    return EditorSyntax.CSharp;
                case ".xml":
                case ".xaml":
                case ".axaml":
                    return EditorSyntax.Xml;
                case ".html":
                case ".htm":
                    return EditorSyntax.Html;
                case ".js":
                case ".mjs":
                case ".cjs":
                    return EditorSyntax.JavaScript;
                case ".Json":
                    return EditorSyntax.Json;
                default:
                    return EditorSyntax.PlainText;
            }
        }

        private void SetSyntaxByExtension(string ext)
        {
            if (RegistryOptions == null || TextMateInstallation == null)
                return;
            var lang = RegistryOptions.GetLanguageByExtension(ext);
            if (lang == null)
            {
                TextMateInstallation.SetGrammar(null);
                return;
            }
            var scope = RegistryOptions.GetScopeByLanguageId(lang.Id);
            TextMateInstallation.SetGrammar(scope);
        }

        private void ApplySyntax(EditorSyntax syntax)
        {
            if (RegistryOptions == null || TextMateInstallation == null)
                return;

            switch (syntax)
            {
                case EditorSyntax.PlainText:
                    SetSyntaxByExtension(".txt");
                    break;
                case EditorSyntax.Markdown:
                    SetSyntaxByExtension(".md");
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
                    SetSyntaxByExtension(".Json");
                    break;
                default:
                    TextMateInstallation.SetGrammar(null);
                    break;
            }
        }

    }
}
