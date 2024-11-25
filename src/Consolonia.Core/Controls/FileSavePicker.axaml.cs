using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePicker : DialogWindow
    {
        public FileSavePicker(FilePickerSaveOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) => { this.FindControl<Button>("CancelButton")?.Focus(); };
            DataContext = new FileSavePickerViewModel(options);
            InitializeComponent();
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
        public FileSavePickerViewModel ViewModel =>
            DataContext as FileSavePickerViewModel
            ?? throw new InvalidOperationException($"Invalid DataContext. Expected {nameof(FileSavePickerViewModel)}");

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
                CloseDialog(file);
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            CloseDialog(ViewModel.SelectedFile);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }
    }
}