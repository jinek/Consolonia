using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePicker : DialogWindow
    {
        public FileSavePicker(FilePickerSaveOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) =>
            {
                this.FindControl<Button>("CancelButton")?.Focus();
            };
            DataContext = new FileSavePickerViewModel(options);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public FilePickerSaveOptions Options => ((FileSavePickerViewModel)DataContext)?.Options ?? throw new ArgumentNullException();

        public FileSavePickerViewModel ViewModel => (FileSavePickerViewModel)DataContext;

        private void OnDoubleTapped(object sender, Avalonia.Input.TappedEventArgs e)
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

        private void OnOK(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CloseDialog(ViewModel.SelectedFile);
        }

        private void OnCancel(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CloseDialog();
        }
    }
}