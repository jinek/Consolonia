using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls
{
    public partial class FileOpenPickerViewModel : PickerViewModel<FilePickerOpenOptions>
    {
        public FileOpenPickerViewModel(FilePickerOpenOptions options)
            : base(options)
        {
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentFolderPath))]
        private int _selectedFilterIndex;

        protected override bool FilterItem(IStorageItem item)
        {
            if (!Options.FileTypeFilter.Any())
                return true;

            if (item is IStorageFile file)
            {
                var selectedFileType = Options.FileTypeFilter[SelectedFilterIndex]!;
                foreach (var pattern in selectedFileType.Patterns)
                {
                    if (file.Path.LocalPath.EndsWith(pattern.TrimStart('*'), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                return false;
            }
            return true;
        }
    }

    public partial class FileOpenPicker : DialogWindow
    {
        public FileOpenPicker(FilePickerOpenOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) =>
            {
                this.FindControl<Button>("Button")?.Focus();
            };
            DataContext = new FileOpenPickerViewModel(options);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnDoubleTapped(object sender, Avalonia.Input.TappedEventArgs e)
        {
            var listbox = (ListBox)sender;
            if (listbox.SelectedItem is SystemStorageFolder folder)
            {
                var model = (FileOpenPickerViewModel)this.DataContext;
                model.CurrentFolder = folder;
                model.CurrentFolderPath = folder.Path.LocalPath;
            }
            else if (listbox.SelectedItem is SystemStorageFile file)
            {
                var model = (FileOpenPickerViewModel)this.DataContext;
                this.CloseDialog(new IStorageFile[] { file });
            }
        }

        private void OnOK(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (FileOpenPickerViewModel)this.DataContext;
            this.CloseDialog(new IStorageFile[] { model.SelectedFile });
        }

        private void OnCancel(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.CloseDialog(null);
        }
    }
}