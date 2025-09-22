using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consolonia.Controls;

namespace Edit.NET
{
    public enum EditorSyntax
    {
        PlainText,
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
        }

        public TextEditor Editor { get; set; }

        [NotifyPropertyChangedFor(nameof(FileName))]
        [NotifyPropertyChangedFor(nameof(FileNameOnly))]
        [NotifyPropertyChangedFor(nameof(Extension))]
        [ObservableProperty]
        private string? _filePath;

        public string? FileName => Path.GetFileName(FilePath);

        public string? FileNameOnly => Path.GetFileNameWithoutExtension(FilePath);

        public string? Extension => Path.GetExtension(FilePath);

        [ObservableProperty]
        private EditorSyntax _syntax = EditorSyntax.PlainText;

        [ObservableProperty]
        private bool _modified;

        [ObservableProperty]
        private string _currentFolder = Environment.CurrentDirectory;


        public async Task OpenFile(string? path)
        {
            FilePath = path;
            Syntax = GetSyntaxFromExtension(Extension);
            Editor.Text = File.Exists(path) ? await File.ReadAllTextAsync(path) : string.Empty;
            Modified = false;
        }

        public async Task SaveFileAsync()
        {
            try
            {
                await File.WriteAllTextAsync(FilePath, Editor.Text);
                Modified = false;
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
        }

        [RelayCommand]
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

        [RelayCommand]
        private async Task SaveAs()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return;
            var mainWindow = desktop.MainWindow;
            var file = await mainWindow.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save As",
                SuggestedFileName = FileName ?? "Untitled.txt"
            });
            if (file != null)
            {
                FilePath = Path.GetFullPath(file.Path.AbsolutePath);
                await SaveFileAsync();
            }
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
                case ".json":
                    return EditorSyntax.Json;
                default:
                    return EditorSyntax.PlainText;
            }
        }
    }
}
