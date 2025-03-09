using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Consolonia.Controls;

namespace Consolonia.Core.Controls
{
    internal partial class FolderPicker : Consolonia.Controls.Window
    {
        public FolderPicker()
            : this(new FolderPickerOpenOptions())
        {
        }

        public FolderPicker(FolderPickerOpenOptions options)
        {
            DataContext = new FolderPickerViewModel(options);
            InitializeComponent();
            CancelButton.Focus();
        }
        
        private FolderPickerViewModel ViewModel => (FolderPickerViewModel)DataContext;

        public FolderPickerOpenOptions Options =>
            ((FolderPickerViewModel)DataContext)?.Options ?? new FolderPickerOpenOptions();

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
            Close(ViewModel.SelectedFolders);
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