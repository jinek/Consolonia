using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit.Indentation.CSharp;
using AvaloniaEdit.TextMate;
using Consolonia;
using Consolonia.Controls;
using EditNET.DataModels;
using EditNET.Helpers;
using EditNET.ViewModels;
using ReactiveUI;
using TextMateSharp.Grammars;
using TextMateSharp.Themes;

namespace EditNET.Views
{
    public partial class EditorView : UserControl
    {
        public const ThemeName DefaultEditorTheme = ThemeName.DimmedMonokai;
        private readonly RegistryOptions _registryOptions;
        private readonly TextMate.Installation _textMateInstallation;

        private CompositeDisposable? _dataContextHandlers;

        public EditorView()
        {
            InitializeComponent();

            Editor.TextArea.IndentationStrategy = new CSharpIndentationStrategy(Editor.Options);
            Editor.TextArea.RightClickMovesCaret = true;
            Editor.AttachedToVisualTree += (_, _) => { UpdateStatus(); };
            Editor.TextArea.Caret.PositionChanged += (_, _) => UpdateStatus();

            _registryOptions = new RegistryOptions(DefaultEditorTheme);
            _textMateInstallation = Editor.InstallTextMate(_registryOptions);
            _textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;
            ApplyThemeColorsToEditor(_textMateInstallation);

            Loaded += OnLoaded;
        }

        private MainWindow MainWindow => this.FindAncestorOfType<MainWindow>()!;

        public EditorViewModel? ViewModel
        {
            get => (EditorViewModel?)DataContext;
            set => DataContext = value;
        }

        private static IClassicDesktopStyleApplicationLifetime Lifetime
            => (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;

        protected override void OnDataContextChanged(EventArgs e)
        {
            _dataContextHandlers?.Dispose();

            base.OnDataContextChanged(e);

            _dataContextHandlers = [];

            if (ViewModel == null) return;
            EditorViewModel editorViewModel = ViewModel!;
            editorViewModel.MessageBoxInteraction.RegisterHandler(MessageBoxHandler).DisposeWith(_dataContextHandlers);
            editorViewModel.FocusEditorInteraction.RegisterHandler(FocusEditorHandler)
                .DisposeWith(_dataContextHandlers);
            editorViewModel.ShutdownInteraction.RegisterHandler(ShutDownHandler).DisposeWith(_dataContextHandlers);
            editorViewModel.OpenFileInteraction.RegisterHandler(OpenFileHandler).DisposeWith(_dataContextHandlers);
            editorViewModel.SaveFileInteraction.RegisterHandler(SaveFileHandler).DisposeWith(_dataContextHandlers);
            editorViewModel.WhenAnyValue(model => model.FilePath).Subscribe(OnFilePathChanged)
                .DisposeWith(_dataContextHandlers);
            editorViewModel.UpdateStatusInteraction.RegisterHandler(context =>
            {
                UpdateStatus();
                context.SetOutput(Unit.Default);
            }).DisposeWith(_dataContextHandlers);
            editorViewModel.Syntax = _registryOptions.GetLanguageByExtension(".txt");
            editorViewModel.WhenAnyValue(model => model.Settings).Subscribe(OnSettingsUpdated)
                .DisposeWith(_dataContextHandlers);
        }

        private void OnSettingsUpdated(Settings settings)
        {
            if (!((ConsoloniaLifetime)Lifetime).IsRgbColorMode())
                return;

            IRawTheme? theme = _registryOptions.LoadTheme(settings.SyntaxTheme);
            _textMateInstallation.SetTheme(theme);
        }

        private void OnFilePathChanged(string? filePath)
        {
            if (filePath == null)
            {
                ViewModel!.Syntax = null;
                _textMateInstallation.SetGrammar(null);
                return;
            }

            Language language =
                ViewModel!.Syntax = _registryOptions.GetLanguageByExtension(Path.GetExtension(filePath));
            string? scope = _registryOptions.GetScopeByLanguageId(language.Id);
            _textMateInstallation.SetGrammar(scope);
        }

        private async Task OpenFileHandler(IInteractionContext<Unit, string?> interactionContext)
        {
            IStorageProvider storageProvider = MainWindow.StorageProvider;
            IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedStartLocation =
                    await storageProvider.TryGetFolderFromPathAsync(Directory.GetCurrentDirectory()),
                Title = "Open File"
            });
            if (files.Count > 0)
            {
                IStorageFile file = files[0];
                interactionContext.SetOutput(file.Path.AbsolutePath);
            }
            else
            {
                interactionContext.SetOutput(null);
            }
        }

        private async Task SaveFileHandler(IInteractionContext<Unit, string?> context)
        {
            IStorageFile? file = await MainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save As",
                SuggestedStartLocation =
                    await MainWindow.StorageProvider.TryGetFolderFromPathAsync(Directory.GetCurrentDirectory()),
                SuggestedFileName = "Untitled.txt"
            });

            context.SetOutput(file?.Path.AbsolutePath);
        }

        private static void ShutDownHandler(IInteractionContext<Unit, Unit> context)
        {
            ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Shutdown();
            context.SetOutput(Unit.Default);
        }

        private async void FocusEditorHandler(IInteractionContext<Unit, Unit> context)
        {
            context.SetOutput(Unit.Default);
            await Task.Delay(500); // todo: low magic number, I don't know how to make focus working
            Dispatcher.UIThread.Post(_ => { Editor.TextArea.Focus(); }, null);
        }

        private static async Task MessageBoxHandler(IInteractionContext<MessageBoxModel, bool> interactionContext)
        {
            MessageBoxResult result = await MessageBox.ShowDialog(interactionContext.Input.Title,
                interactionContext.Input.Message,
                interactionContext.Input.Buttons switch
                {
                    MessageBoxButtons.OkCancel => MessageBoxStyle.OkCancel,
                    MessageBoxButtons.YesNo => MessageBoxStyle.YesNoCancel,
                    MessageBoxButtons.Ok => MessageBoxStyle.Ok,
                    _ => throw new NotSupportedException("Unsupported MessageBoxButtons value: " +
                                                         interactionContext.Input.Buttons)
                });
            interactionContext.SetOutput(result is MessageBoxResult.Ok or MessageBoxResult.Yes);
        }

        private void UpdateStatus()
        {
            // Position
            int line = Editor.TextArea.Caret.Line;
            int column = Editor.TextArea.Caret.Column;
            PositionText.Text = $"Ln {line}, Col {column}";
            // Length
            int length = Editor.Document?.TextLength ?? (Editor.Text?.Length ?? 0);
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

            if (!ApplyBrushAction(e, "editor.selectionBackground",
                    brush => Editor.TextArea.SelectionBrush = brush))
                if (!ApplyBrushAction(e, "editor.selectionHighlightBackground",
                        brush => Editor.TextArea.SelectionBrush = brush))
                    if (Application.Current!.TryGetResource("TextAreaSelectionBrush", out object? resourceObject))
                        if (resourceObject is IBrush brush)
                            Editor.TextArea.SelectionBrush = brush;

            if (!ApplyBrushAction(e, "editor.lineHighlightBackground",
                    brush => { Editor.TextArea.TextView.CurrentLineBackground = brush; }))
                Editor.TextArea.TextView.SetDefaultHighlightLineColors();

            //Todo: looks like the margin doesn't have a active line highlight, would be a nice addition
            if (!ApplyBrushAction(e, "editorLineNumber.foreground",
                    brush => Editor.LineNumbersForeground = brush))
                Editor.LineNumbersForeground = Editor.TextArea.Foreground;
            Editor.TextArea.TextView.CurrentLineBorder = new Pen(Brushes.Transparent, 0);
            return;

            static bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson,
                Action<IBrush> applyColorAction)
            {
                if (!e.TryGetThemeColor(colorKeyNameFromJson, out string? colorString))
                    return false;
                if (!Color.TryParse(colorString, out Color color))
                    return false;

                var colorBrush = new SolidColorBrush(color);
                applyColorAction(colorBrush);
                return true;
            }
        }

        private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog(this);
        }

        private async void OnShowSettings(object? sender, RoutedEventArgs e)
        {
            var dlg = new EditSettingsDialog(ViewModel!.Settings.SerializedCopy());
            var newSettings = await dlg.ShowDialog<Settings?>(this);
            if (newSettings != null)
            {
                ViewModel.Settings = newSettings;
                Editor.TextArea.Focus();
            }
        }

        private void EditMenu_OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            foreach (MenuItem subMenu in ((MenuItem)sender).Items.OfType<MenuItem>())
                BindingOperations.GetBindingExpressionBase(subMenu, IsEnabledProperty)?.UpdateTarget();
        }
    }
}