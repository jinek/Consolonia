using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Consolonia.Controls
{
    public partial class FolderPicker : DialogWindow
    {
        public FolderPicker()
            : this(new FolderPickerOpenOptions())
        {
        }

        public FolderPicker(FolderPickerOpenOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            DataContext = new FolderPickerViewModel(options);
            InitializeComponent();
            CancelButton.Focus();
        }

        private FolderPickerViewModel ViewModel => (FolderPickerViewModel)DataContext;

        public FolderPickerOpenOptions Options =>
            ((FolderPickerViewModel)DataContext)?.Options ?? new FolderPickerOpenOptions();

        private void OnDoubleTapped(object sender, TappedEventArgs e)
        {
            var listbox = (ListBox)sender;
            if (listbox.SelectedItem is SystemStorageFolder folder)
            {
                ViewModel.CurrentFolder = folder;
                ViewModel.CurrentFolderPath = folder.Path.LocalPath;
                ViewModel.SelectedFolders.Clear();
                ViewModel.HasSelection = false;
            }
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            CloseDialog(ViewModel.SelectedFolders);
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
                    e.AddedItems[0] is IStorageFolder folder)
                {
                    ViewModel.SelectedFolders.Clear();
                    ViewModel.SelectedFolders.Add(folder);
                }
            }
            else
            {
                foreach (object item in e.AddedItems)
                    if (item is IStorageFolder folder)
                        ViewModel.SelectedFolders.Add(folder);

                foreach (object item in e.RemovedItems)
                    if (item is IStorageFolder folder)
                        ViewModel.SelectedFolders.Remove(folder);
            }

            ViewModel.HasSelection = ViewModel.SelectedFolders.Count > 0;
        }
    }
}