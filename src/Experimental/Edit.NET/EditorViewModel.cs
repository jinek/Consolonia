using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
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
            FilePath = Path.Combine(CurrentFolder, $"Untitled.txt");
        }

        [NotifyPropertyChangedFor(nameof(FileName))]
        [NotifyPropertyChangedFor(nameof(FileNameOnly))]
        [NotifyPropertyChangedFor(nameof(Extension))]
        [ObservableProperty]
        private string? _filePath;

        public string? FileName => Path.GetFileName(FilePath);

        public string? FileNameOnly => Path.GetFileNameWithoutExtension(FilePath);

        public string? Extension => Path.GetFileNameWithoutExtension(FilePath);

        [ObservableProperty]
        private EditorSyntax _syntax = EditorSyntax.PlainText;

        [ObservableProperty]
        private bool _modified;

        [ObservableProperty]
        private string _text = String.Empty;

        [ObservableProperty]
        private string _currentFolder = Environment.CurrentDirectory;

        public void NewFile()
        {
            NewFile("Untitled", ".txt");
        }

        public void NewFile(string name, string ext)
        {
            FilePath = Path.Combine(CurrentFolder, $"{name}.{ext.TrimStart('.')}");
            Modified = false;
            Text = string.Empty;
        }

        public async Task OpenFile(string? path)
        {
            FilePath = path;
            Syntax = GetSyntaxFromExtension(Extension);
            Text = File.Exists(path) ? await File.ReadAllTextAsync(path) : string.Empty;
            Modified = false;
        }

        public async Task Save()
        {
            try
            {
                await File.WriteAllTextAsync(FilePath, Text);
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

        public async Task SaveAs(string fullPath)
        {
            this.FilePath = fullPath;
            await Save();
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
