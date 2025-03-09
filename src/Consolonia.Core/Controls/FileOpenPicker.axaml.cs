using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls
{
    internal partial class FileOpenPicker : Consolonia.Controls.Window
    {
        public FileOpenPicker()
            : this(new FilePickerOpenOptions())
        {
        }

        public FileOpenPicker(FilePickerOpenOptions options)
        {
            DataContext = new FileOpenPickerViewModel(options);
            InitializeComponent();
            CancelButton.Focus();
        }

        /// <summary>
        ///     Gets the view model associated with this picker.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the DataContext is null or not of type FileOpenPickerViewModel.</exception>
        private FileOpenPickerViewModel ViewModel =>
            DataContext as FileOpenPickerViewModel
            ?? throw new InvalidOperationException("DataContext is not properly initialized.");

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            Position = new PixelPoint(2, 2);
            Width = OverlayLayer.Bounds.Width - 4;
            Height = OverlayLayer.Bounds.Height - 4;
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
                Close(new[] { file });
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            Close(ViewModel.SelectedFiles);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
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