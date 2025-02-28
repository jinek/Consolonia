using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Consolonia.Controls
{
    public partial class FileSavePicker : DialogWindow
    {
        public FileSavePicker()
            : this(new FilePickerSaveOptions())
        {
        }

        public FileSavePicker(FilePickerSaveOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

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
                CloseDialog(file);
            }
        }

        private async void OnOK(object sender, RoutedEventArgs e)
        {
            var storageProvider = TopLevel.GetTopLevel(this).StorageProvider;
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
                    CloseDialog();
                    return;
                }

                file = await folder.CreateFileAsync(Path.GetFileName(savePath));
            }

            CloseDialog(file);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }
    }
}