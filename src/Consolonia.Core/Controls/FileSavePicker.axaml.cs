using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Window = Consolonia.Controls.Window;

namespace Consolonia.Core.Controls
{
    internal partial class FileSavePicker : Window
    {
        public FileSavePicker()
            : this(new FilePickerSaveOptions())
        {
        }

        public FileSavePicker(FilePickerSaveOptions options)
        {
            DataContext = new FileSavePickerViewModel(options);
            InitializeComponent();
            CancelButton.Focus();
        }

        /// <summary>
        ///     Gets the file picker save options associated with this dialog.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when DataContext is null or of incorrect type.</exception>
        public FilePickerSaveOptions Options =>
            (DataContext as FileSavePickerViewModel)?.Options
            ?? throw new InvalidOperationException($"Invalid DataContext. Expected {nameof(FileSavePickerViewModel)}");

        /// <summary>
        ///     Gets the view model associated with this dialog.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when DataContext is null or of incorrect type.</exception>
        private FileSavePickerViewModel ViewModel =>
            DataContext as FileSavePickerViewModel
            ?? throw new InvalidOperationException($"Invalid DataContext. Expected {nameof(FileSavePickerViewModel)}");

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            Position = new PixelPoint(2, 2);
            Width = WindowsPanel.Bounds.Width - 4;
            Height = WindowsPanel.Bounds.Height - 4;
        }

        private void OnDoubleTapped(object sender, TappedEventArgs e)
        {
            var listbox = (ListBox)sender;
            if (listbox.SelectedItem is IStorageFolder folder)
            {
                ViewModel.CurrentFolder = folder;
                ViewModel.CurrentFolderPath = folder.Path.LocalPath;
            }
            else if (listbox.SelectedItem is IStorageFile file)
            {
                Close(file);
            }
        }

        private async void OnOK(object sender, RoutedEventArgs e)
        {
            IStorageProvider storageProvider = TopLevel.GetTopLevel(this).StorageProvider;
            ArgumentNullException.ThrowIfNull(storageProvider);

            string savePath = ViewModel.SavePath;
            if (!Path.IsPathFullyQualified(ViewModel.SavePath))
                savePath = Path.GetFullPath(Path.Combine(ViewModel.CurrentFolder.Path.LocalPath, ViewModel.SavePath));

            IStorageFile file =
                await storageProvider.TryGetFileFromPathAsync(new Uri($"file://{savePath}"));
            if (file == null)
            {
                IStorageFolder folder =
                    await storageProvider.TryGetFolderFromPathAsync(
                        new Uri($"file://{Path.GetDirectoryName(savePath)}"));
                if (folder == null)
                {
                    Close();
                    return;
                }

                file = await folder.CreateFileAsync(Path.GetFileName(savePath));
            }

            Close(file);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}