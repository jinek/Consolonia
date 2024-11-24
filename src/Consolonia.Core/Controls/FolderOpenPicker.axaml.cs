using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls
{
    public class FolderPickerViewModel : PickerViewModel<FolderPickerOpenOptions>
    {
        public FolderPickerViewModel(FolderPickerOpenOptions options)
            : base(options)
        {
        }

        protected override bool FilterItem(IStorageItem item)
        {
            return item is IStorageFolder;
        }
    }

    public partial class FolderOpenPicker : DialogWindow
    {
        public FolderOpenPicker(FolderPickerOpenOptions options)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Loaded += (_, _) =>
            {
                this.FindControl<Button>("Button")?.Focus();
            };
            DataContext = new FolderPickerViewModel(options);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public FolderPickerOpenOptions Options => ((FolderPickerViewModel)DataContext).Options;

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
            this.CloseDialog(new IStorageFolder[] { model.SelectedFolder });
        }

        private void OnCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.CloseDialog(null);
        }
    }
}