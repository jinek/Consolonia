using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Edit.NET
{
    public partial class MainWindow : Window
    {
        private Avalonia.Platform.Storage.IStorageFile? _currentFile;
        private string? _currentFileName;
        private bool _isModified;

        public MainWindow()
        {
            InitializeComponent();
            // Wire up editor events for status updates
            Editor.TextChanged += (_, __) => { _isModified = true; UpdateStatus(); };
            Editor.AttachedToVisualTree += (_, __) => UpdateStatus();
            Editor.TextArea.Caret.PositionChanged += (_, __) => UpdateStatus();
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
            // No-op for now; plain text (no syntax highlighting)
        }

        private void SyntaxCSharp_OnClick(object? sender, RoutedEventArgs e)
        {
            // Placeholder: Syntax highlighting not implemented in this sample
        }

        private void SyntaxXml_OnClick(object? sender, RoutedEventArgs e)
        {
            // Placeholder
        }

        private void SyntaxHtml_OnClick(object? sender, RoutedEventArgs e)
        {
            // Placeholder
        }

        private void SyntaxJavaScript_OnClick(object? sender, RoutedEventArgs e)
        {
            // Placeholder
        }
    }
}