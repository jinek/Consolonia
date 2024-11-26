using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls
{
    public partial class FileOpenPicker : DialogWindow
    {
        public FileOpenPicker()
            : this(new FilePickerOpenOptions())
        {
        }

        public FileOpenPicker(FilePickerOpenOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) => { this.FindControl<Button>("CancelButton")?.Focus(); };
            DataContext = new FileOpenPickerViewModel(options);
            InitializeComponent();
        }

        /// <summary>
        ///     Gets the view model associated with this picker.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the DataContext is null or not of type FileOpenPickerViewModel.</exception>
        private FileOpenPickerViewModel ViewModel =>
            DataContext as FileOpenPickerViewModel
            ?? throw new InvalidOperationException("DataContext is not properly initialized.");

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
                ViewModel.SelectedFiles.Clear();
            }
            else if (listbox.SelectedItem is IStorageFile file)
            {
                CloseDialog(new[] { file });
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            CloseDialog(ViewModel.SelectedFiles);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectionMode == SelectionMode.Single)
            {
                if (e.AddedItems.Count > 0 &&
                    e.AddedItems[0] is IStorageFile file)
                {
                    ViewModel.SelectedFiles.Clear();
                    ViewModel.SelectedFiles.Add(file);
                }
            }
            else
            {
                foreach (object item in e.AddedItems)
                    if (item is IStorageFile file)
                        ViewModel.SelectedFiles.Add(file);

                foreach (object item in e.RemovedItems)
                    if (item is IStorageFile file)
                        ViewModel.SelectedFiles.Remove(file);
            }

            ViewModel.HasSelection = ViewModel.SelectedFiles.Count > 0;
        }
    }
}