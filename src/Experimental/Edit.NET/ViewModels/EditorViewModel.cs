using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using EditNET.DataModels;
using JetBrains.Annotations;
using ReactiveUI;
using TextMateSharp.Grammars;

namespace EditNET.ViewModels
{
    public partial class EditorViewModel : ObservableObject
    {
        [ObservableProperty] private TextDocument _document = new();
        [ObservableProperty] private string? _filePath;
        [ObservableProperty] private bool _modified;
        private TextDocument? _previousDocument;
        [ObservableProperty] private Settings _settings;
        [ObservableProperty] private Language? _syntax;

        public EditorViewModel(Settings settings)
        {
            _settings = settings;
            this.WhenAnyValue(model => model.Document).Subscribe(OnDocumentUpdated);
            this.WhenAnyValue(model => model.Document).Skip(1).Subscribe(OnDocumentUpdatedNoInitial);
        }

        [UsedImplicitly]
        public EditorViewModel()
        {
            _settings = new Settings();
        }

        public Interaction<MessageBoxModel, bool> MessageBoxInteraction { get; } = new();
        public Interaction<Unit, Unit> FocusEditorInteraction { get; } = new();
        public Interaction<Unit, string?> OpenFileInteraction { get; } = new();
        public Interaction<Unit, string?> SaveFileInteraction { get; } = new();
        public Interaction<Unit, Unit> ShutdownInteraction { get; } = new();
        public Interaction<Unit, Unit> UpdateStatusInteraction { get; } = new();

        private async void OnDocumentUpdatedNoInitial(TextDocument newDocument)
        {
            await UpdateStatusInteraction.Handle(Unit.Default);
        }

        private void OnDocumentUpdated(TextDocument newDocument)
        {
            if (_previousDocument != null) _previousDocument.TextChanged -= NewDocumentOnTextChanged;
            _previousDocument = newDocument;
            Modified = false;
            newDocument.TextChanged += NewDocumentOnTextChanged;
        }

        private void NewDocumentOnTextChanged(object? sender, EventArgs e)
        {
            Modified = true;
            UpdateStatusInteraction.Handle(Unit.Default).Wait();
        }

        public async Task NewCommand()
        {
            if (!await CheckSaved())
                return;

            Document = new TextDocument();
            FilePath = null;

            await FocusEditorInteraction.Handle(Unit.Default);
        }

        public async Task OpenCommand()
        {
            if (!await CheckSaved())
                return;

            string? filePath = await OpenFileInteraction.Handle(Unit.Default);
            if (filePath == null)
                return;

            await OpenFile(Path.GetFullPath(filePath));

            await FocusEditorInteraction.Handle(Unit.Default);
        }

        public async Task SaveCommand()
        {
            if (FilePath == null)
                await SaveAsCommand();
            else await SaveFileInternalAsync();

            await FocusEditorInteraction.Handle(Unit.Default);
        }

        public async Task SaveAsCommand()
        {
            string? filePath = await SaveFileInteraction.Handle(Unit.Default);
            if (filePath == null)
                return;

            FilePath = Path.GetFullPath(filePath);
            await SaveFileInternalAsync();

            await FocusEditorInteraction.Handle(Unit.Default);
        }

        public async Task ExitCommand()
        {
            if (!await CheckSaved())
                return;
            await ShutdownInteraction.Handle(Unit.Default);
        }

        public async Task OpenFile(string path)
        {
            FilePath = path;
            bool opened = false;
            await HandleFileExceptions(async () =>
            {
                Document = new TextDocument(new StringTextSource(await File.ReadAllTextAsync(path)));
                opened = true;
            });

            if (opened)
                Directory.SetCurrentDirectory(Path.GetDirectoryName(path)!);
        }

        private async Task SaveFileInternalAsync()
        {
            await HandleFileExceptions(async () => { await File.WriteAllTextAsync(FilePath!, Document.Text); });
            Modified = false;
            Directory.SetCurrentDirectory(Path.GetDirectoryName(FilePath!)!);
        }

        private async Task<bool> CheckSaved()
        {
            if (Modified)
                return await MessageBoxInteraction.Handle(new MessageBoxModel("Unsaved Changes",
                    "You have unsaved changes. Do you want to discard them?", MessageBoxButtons.YesNo));

            return true;
        }

        private async Task HandleFileExceptions(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                await MessageBoxInteraction.Handle(new MessageBoxModel("File Access Exception", exception.Message,
                    MessageBoxButtons.Ok));
            }
        }
    }
}