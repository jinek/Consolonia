using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using ConsoloniaEdit.Demo.Resources;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Folding;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.TextMate;
using TextMateSharp.Grammars;
using AvaloniaEdit.Snippets;
using Snippet = AvaloniaEdit.Snippets.Snippet;
using ConsoloniaEdit.Demo.ViewModels;
using AvaloniaEdit;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
namespace ConsoloniaEdit.Demo
{
    using Pair = KeyValuePair<int, Control>;

    public partial class MainWindow : Window
    {
        private readonly TextEditor _textEditor;
        private FoldingManager _foldingManager;
        private readonly TextMate.Installation _textMateInstallation;
        private CompletionWindow _completionWindow;
        private OverloadInsightWindow _insightWindow;
        private ComboBox _syntaxModeCombo;
        private TextBlock _statusTextBlock;
        private ElementGenerator _generator = new ElementGenerator();
        private RegistryOptions _registryOptions;
        private int _currentTheme = (int)ThemeName.DarkPlus;

        public MainWindow()
        {
            InitializeComponent();

            // this.AttachDevTools();

            _textEditor = this.FindControl<TextEditor>("Editor")!;
            _textEditor.HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible;
            _textEditor.Background = Brushes.Transparent;
            _textEditor.ShowLineNumbers = true;
            _textEditor.TextArea.Background = this.Background;
            _textEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            _textEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            _textEditor.Options.AllowToggleOverstrikeMode = true;
            _textEditor.Options.EnableTextDragDrop = true;
            _textEditor.Options.ShowBoxForControlCharacters = true;
            _textEditor.Options.HighlightCurrentLine = false;
            _textEditor.TextArea.IndentationStrategy = new AvaloniaEdit.Indentation.CSharp.CSharpIndentationStrategy(_textEditor.Options);
            _textEditor.TextArea.Caret.PositionChanged += Caret_PositionChanged;
            _textEditor.TextArea.RightClickMovesCaret = true;

            //_addControlButton = this.FindControl<Button>("addControlBtn");
            //_addControlButton.Click += AddControlButton_Click;

            //_clearControlButton = this.FindControl<Button>("clearControlBtn");
            //_clearControlButton.Click += ClearControlButton_Click;

            //_insertSnippetButton = this.FindControl<Button>("insertSnippetBtn");
            //_insertSnippetButton.Click += InsertSnippetButton_Click;

            _textEditor.TextArea.TextView.ElementGenerators.Add(_generator);

            _registryOptions = new RegistryOptions(
                (ThemeName)_currentTheme);

            _textMateInstallation = _textEditor.InstallTextMate(_registryOptions);

            _textMateInstallation.AppliedTheme += TextMateInstallationOnAppliedTheme;

            Language csharpLanguage = _registryOptions.GetLanguageByExtension(".cs");

            _syntaxModeCombo = this.FindControl<ComboBox>("syntaxModeCombo")!;
            _syntaxModeCombo.ItemsSource = _registryOptions.GetAvailableLanguages();
            _syntaxModeCombo.SelectedItem = csharpLanguage;
            _syntaxModeCombo.SelectionChanged += SyntaxModeCombo_SelectionChanged;

            string scopeName = _registryOptions.GetScopeByLanguageId(csharpLanguage.Id);

            _textEditor.Document = new TextDocument(ResourceLoader.LoadSampleFile(scopeName));
            _textMateInstallation.SetGrammar(_registryOptions.GetScopeByLanguageId(csharpLanguage.Id));

            _statusTextBlock = this.Find<TextBlock>("StatusText")!;

            this.AddHandler(PointerWheelChangedEvent, (o, i) =>
            {
                if (i.KeyModifiers != KeyModifiers.Control) return;
                if (i.Delta.Y > 0) _textEditor.FontSize++;
                else _textEditor.FontSize = _textEditor.FontSize > 1 ? _textEditor.FontSize - 1 : 1;
            }, RoutingStrategies.Bubble, true);

            var mainWindowVM = new MainWindowViewModel(_textMateInstallation, _registryOptions);
            foreach (ThemeName themeName in Enum.GetValues<ThemeName>())
            {
                var themeViewModel = new ThemeViewModel(themeName);
                mainWindowVM.AllThemes.Add(themeViewModel);
                if (themeName == ThemeName.LightPlus)
                {
                    mainWindowVM.SelectedTheme = themeViewModel;
                }
            }
            DataContext = mainWindowVM;


        }

        private void TextMateInstallationOnAppliedTheme(object sender, TextMate.Installation e)
        {
            ApplyThemeColorsToEditor(e);
            ApplyThemeColorsToWindow(e);
        }

        void ApplyThemeColorsToEditor(TextMate.Installation e)
        {
            ApplyBrushAction(e, "editor.background", brush => _textEditor.Background = brush);
            ApplyBrushAction(e, "editor.foreground", brush => _textEditor.Foreground = brush);

            if (!ApplyBrushAction(e, "editor.selectionBackground",
                    brush => _textEditor.TextArea.SelectionBrush = brush))
            {
                if (Application.Current!.TryGetResource("TextAreaSelectionBrush", out var resourceObject))
                {
                    if (resourceObject is IBrush brush)
                    {
                        _textEditor.TextArea.SelectionBrush = brush;
                    }
                }
            }

            if (!ApplyBrushAction(e, "editor.lineHighlightBackground",
                    brush =>
                    {
                        _textEditor.TextArea.TextView.CurrentLineBackground = brush;
                        _textEditor.TextArea.TextView.CurrentLineBorder = new Pen(brush); // Todo: VS Code didn't seem to have a border but it might be nice to have that option. For now just make it the same..
                    }))
            {
                _textEditor.TextArea.TextView.SetDefaultHighlightLineColors();
            }

            //Todo: looks like the margin doesn't have a active line highlight, would be a nice addition
            if (!ApplyBrushAction(e, "editorLineNumber.foreground",
                    brush => _textEditor.LineNumbersForeground = brush))
            {
                _textEditor.LineNumbersForeground = _textEditor.Foreground;
            }

            // disable border around selection
            _textEditor.TextArea.SelectionBorder = null;  
            _textEditor.TextArea.SelectionCornerRadius = 0;
        }

        private void ApplyThemeColorsToWindow(TextMate.Installation e)
        {
            var panel = this.Find<StackPanel>("StatusBar");
            if (panel == null)
            {
                return;
            }

            if (!ApplyBrushAction(e, "statusBar.background", brush => panel.Background = brush))
            {
                panel.Background = Brushes.Purple;
            }

            if (!ApplyBrushAction(e, "statusBar.foreground", brush => _statusTextBlock.Foreground = brush))
            {
                _statusTextBlock.Foreground = Brushes.White;
            }

            //Applying the Editor background to the whole window for demo sake.
            ApplyBrushAction(e, "editor.background", brush => Background = brush);
            ApplyBrushAction(e, "editor.foreground", brush => Foreground = brush);
        }

        bool ApplyBrushAction(TextMate.Installation e, string colorKeyNameFromJson, Action<IBrush> applyColorAction)
        {
            if (!e.TryGetThemeColor(colorKeyNameFromJson, out var colorString))
                return false;

            if (!Color.TryParse(colorString, out Color color))
                return false;

            var colorBrush = new SolidColorBrush(color);
            applyColorAction(colorBrush);
            return true;
        }

        private void Caret_PositionChanged(object sender, EventArgs e)
        {
            _statusTextBlock.Text = string.Format("Line {0} Column {1}",
                _textEditor.TextArea.Caret.Line,
                _textEditor.TextArea.Caret.Column);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _textMateInstallation.Dispose();
        }

        private void SyntaxModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Language language = (Language)_syntaxModeCombo.SelectedItem;

            if (_foldingManager != null)
            {
                _foldingManager.Clear();
                FoldingManager.Uninstall(_foldingManager);
            }

            string scopeName = _registryOptions.GetScopeByLanguageId(language.Id);

            _textMateInstallation.SetGrammar(null);
            _textEditor.Document = new TextDocument(ResourceLoader.LoadSampleFile(scopeName));
            _textMateInstallation.SetGrammar(scopeName);

            if (language.Id == "xml")
            {
                _foldingManager = FoldingManager.Install(_textEditor.TextArea);

                var strategy = new XmlFoldingStrategy();
                strategy.UpdateFoldings(_foldingManager, _textEditor.Document);
                return;
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void textEditor_TextArea_TextEntering(object sender, TextInputEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }

            _insightWindow?.Hide();

            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }

        private void textEditor_TextArea_TextEntered(object sender, TextInputEventArgs e)
        {
            if (e.Text == ".")
            {

                _completionWindow = new CompletionWindow(_textEditor.TextArea);
                _completionWindow.Closed += (o, args) => _completionWindow = null;

                var data = _completionWindow.CompletionList.CompletionData;

                for (int i = 0; i < 500; i++)
                {
                    data.Add(new MyCompletionData("Item" + i.ToString()));
                }

                data.Insert(20, new MyCompletionData("long item to demosntrate dynamic poup resizing"));

                _completionWindow.Show();
            }
            else if (e.Text == "(")
            {
                _insightWindow = new OverloadInsightWindow(_textEditor.TextArea);
                _insightWindow.Closed += (o, args) => _insightWindow = null;

                _insightWindow.Provider = new MyOverloadProvider(new[]
                {
                    ("Method1(int, string)", "Method1 description"),
                    ("Method2(int)", "Method2 description"),
                    ("Method3(string)", "Method3 description"),
                });

                _insightWindow.Show();
            }
        }


        private class MyOverloadProvider : IOverloadProvider
        {
            private readonly IList<(string header, string content)> _items;
            private int _selectedIndex;

            public MyOverloadProvider(IList<(string header, string content)> items)
            {
                _items = items;
                SelectedIndex = 0;
            }

            public int SelectedIndex
            {
                get => _selectedIndex;
                set
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                    // ReSharper disable ExplicitCallerInfoArgument
                    OnPropertyChanged(nameof(CurrentHeader));
                    OnPropertyChanged(nameof(CurrentContent));
                    // ReSharper restore ExplicitCallerInfoArgument
                }
            }

            public int Count => _items.Count;
            public string CurrentIndexText => $"{SelectedIndex + 1} of {Count}";
            public object CurrentHeader => _items[SelectedIndex].header;
            public object CurrentContent => _items[SelectedIndex].content;

            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class MyCompletionData : ICompletionData
        {
            public MyCompletionData(string text)
            {
                Text = text;
            }

            public IImage Image => null;

            public string Text { get; }

            // Use this property if you want to show a fancy UIElement in the list.
            public object Content => _contentControl ??= BuildContentControl();

            public object Description => "Description for " + Text;

            public double Priority { get; } = 0;

            public void Complete(TextArea textArea, ISegment completionSegment,
                EventArgs insertionRequestEventArgs)
            {
                textArea.Document.Replace(completionSegment, Text);
            }

            Control BuildContentControl()
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = Text;
                textBlock.Margin = new Thickness(5);

                return textBlock;
            }

            Control _contentControl;
        }

        class ElementGenerator : VisualLineElementGenerator, IComparer<Pair>
        {
            public List<Pair> controls = new List<Pair>();

            /// <summary>
            /// Gets the first interested offset using binary search
            /// </summary>
            /// <returns>The first interested offset.</returns>
            /// <param name="startOffset">Start offset.</param>
            public override int GetFirstInterestedOffset(int startOffset)
            {
                int pos = controls.BinarySearch(new Pair(startOffset, null), this);
                if (pos < 0)
                    pos = ~pos;
                if (pos < controls.Count)
                    return controls[pos].Key;
                else
                    return -1;
            }

            public override VisualLineElement ConstructElement(int offset)
            {
                int pos = controls.BinarySearch(new Pair(offset, null), this);
                if (pos >= 0)
                    return new InlineObjectElement(0, controls[pos].Value);
                else
                    return null;
            }

            int IComparer<Pair>.Compare(Pair x, Pair y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }

        private void InsertSnippetButton_Click(object sender, RoutedEventArgs e)
        {
            var className = new SnippetReplaceableTextElement { Text = "Name" };
            var snippet = new Snippet
            {
                Elements =
                {
                    new SnippetTextElement { Text = "public class " },
                    className,
                    new SnippetTextElement
                    {
                        Text = "\n{\n    public "
                    },
                    new SnippetBoundElement { TargetElement = className },
                    new SnippetTextElement { Text = "()\n    {\n        " },
                    new SnippetCaretElement(),
                    new SnippetTextElement { Text = "\n    }\n}" }
                }
            };

            snippet.Insert(_textEditor.TextArea);
            _textEditor.Focus();
        }
    }
}
