using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Controls.Dialog;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls
{
    public partial class FileSavePickerViewModel : PickerViewModel<FilePickerSaveOptions>
    {
        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {
        }

        [ObservableProperty]
        private FilePickerFileType? _selectedFileType;

        protected override bool FilterItem(IStorageItem item)
        {
            if (_selectedFileType != null)
            {
                if (item is IStorageFolder folder)
                {
                    return true;
                }
                if (item is IStorageFile file)
                {
                    foreach (var pattern in _selectedFileType.Patterns)
                    {
                        if (file.Path.LocalPath.EndsWith(pattern.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    return false;
                }
            }
            return true;
        }
    }
    public partial class FileSavePicker : DialogWindow
    {
        public FileSavePicker(FilePickerSaveOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) =>
            {
                this.FindControl<Button>("Button")?.Focus();
            };
            DataContext = new FileSavePickerViewModel(options);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public FilePickerSaveOptions Options => ((FileSavePickerViewModel)DataContext).Options;

        private void OnDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var listbox = (ListBox)sender;
            if (listbox.SelectedItem is SystemStorageFolder folder)
            {
                var model = (FileSavePickerViewModel)this.DataContext;
                model.CurrentFolder = folder;
                model.CurrentFolderPath = folder.Path.LocalPath;
            }
            else if (listbox.SelectedItem is SystemStorageFile file)
            {
                var model = (FileSavePickerViewModel)this.DataContext;
                this.CloseDialog(new IStorageFile[] { file });
            }
        }

        private void OnOK(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (FileSavePickerViewModel)this.DataContext;
            this.CloseDialog(new IStorageFile[] { (IStorageFile)model.SelectedItem });
        }

        private void OnCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.CloseDialog(null);
        }
    }
}