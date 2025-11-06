using Avalonia;
using Avalonia.Controls;
using Avalonia.Dialogs.Internal;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Iciclecreek.Avalonia.WindowManager;
using NLog.Filters;

namespace Consolonia.Core.Controls
{
    internal partial class FolderPicker : ManagedWindow
    {
        public FolderPicker()
            : this(new FolderPickerOpenOptions())
        {
        }

        public FolderPicker(FolderPickerOpenOptions options)
        {
            DataContext = new FolderPickerViewModel(options);
            InitializeComponent();

            CurrentFolderTextBox.Focus();
            ItemsListBox.KeepFocus(() => !CurrentFolderTextBox.IsFocused);
        }

        private FolderPickerViewModel ViewModel => (FolderPickerViewModel)DataContext;

        public FolderPickerOpenOptions Options =>
            ((FolderPickerViewModel)DataContext)?.Options ?? new FolderPickerOpenOptions();

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            Position = new PixelPoint(2, 2);
            Width = WindowsPanel.Bounds.Width - 4;
            Height = WindowsPanel.Bounds.Height - 4;
        }

        private void OnDoubleTapped(object sender, TappedEventArgs e)
        {
            if (ItemsListBox.SelectedItem is SystemStorageFolder folder)
            {
                ViewModel.CurrentFolder = folder;
                ViewModel.CurrentFolderPath = folder.Path.LocalPath;
                ViewModel.SelectedFolders.Clear();
                if (folder.Name != "..")
                    ViewModel.SelectedFolders.Add(folder);
                e.Handled = true;
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            var focusedListBoxItem = ItemsListBox.GetFocusedListBoxItem();
            if (focusedListBoxItem != null)
            {
                var item = ItemsListBox.ItemFromContainer(focusedListBoxItem);
                if (item is IStorageFolder folder)
                {
                    ViewModel.CurrentFolder = folder;
                    ViewModel.CurrentFolderPath = folder.Path.LocalPath;
                    ViewModel.SelectedFolders.Clear();
                    return;
                }
            }

            if (ViewModel.SelectionMode == SelectionMode.Single &&
                !ViewModel.HasSelection &&
                ViewModel.CurrentFolder != null)
            {
                if (ViewModel.CurrentFolder.Name != "..")
                    ViewModel.SelectedFolders.Add(ViewModel.CurrentFolder);
            }


            if (ViewModel.HasSelection)
            {
                Close(ViewModel.SelectedFolders);
                return;
            }
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
                    e.AddedItems[0] is IStorageFolder folder)
                {
                    ViewModel.SelectedFolders.Clear();
                    if (folder.Name != "..")
                        ViewModel.SelectedFolders.Add(folder);
                }
            }
            else
            {
                foreach (object item in e.AddedItems)
                    if (item is IStorageFolder folder && 
                        folder.Name != "..")
                        ViewModel.SelectedFolders.Add(folder);

                foreach (object item in e.RemovedItems)
                    if (item is IStorageFolder folder)
                        ViewModel.SelectedFolders.Remove(folder);
            }
        }
    }
}