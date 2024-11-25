using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls
{

    public partial class FolderPicker : DialogWindow
    {
        public FolderPicker(FolderPickerOpenOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) =>
            {
                this.FindControl<Button>("CancelButton")?.Focus();
            };
            DataContext = new FolderPickerViewModel(options);
            InitializeComponent();
        }

        public FolderPickerViewModel ViewModel => (FolderPickerViewModel)DataContext;

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public FolderPickerOpenOptions Options => ((FolderPickerViewModel)DataContext).Options;

        private void OnDoubleTapped(object sender, Avalonia.Input.TappedEventArgs e)
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

        private void OnOK(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CloseDialog(ViewModel.SelectedFolders);
        }

        private void OnCancel(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CloseDialog();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.SelectionMode == SelectionMode.Single)
            {
                if ((e.AddedItems.Count > 0) &&
                    (e.AddedItems[0] is IStorageFolder folder))
                {
                    ViewModel.SelectedFolders.Clear();
                    ViewModel.SelectedFolders.Add(folder);
                }
            }
            else
            {
                foreach (var item in e.AddedItems)
                {
                    if (item is IStorageFolder folder)
                        ViewModel.SelectedFolders.Add(folder);
                }

                foreach (var item in e.RemovedItems)
                {
                    if (item is IStorageFolder folder)
                        ViewModel.SelectedFolders.Remove(folder);
                }
            }
            ViewModel.HasSelection = ViewModel.SelectedFolders.Count > 0;
        }
    }
}