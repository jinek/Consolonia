using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Consolonia.Core.Controls.Dialog;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls
{
    public class FileOpenPickerViewModel : PickerViewModel<FilePickerOpenOptions>
    {
        public FileOpenPickerViewModel(FilePickerOpenOptions options)
            : base(options)
        {
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

        public FilePickerOpenOptions Options => ((FileOpenPickerViewModel)DataContext).Options;

        private void OnDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
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

        private void OnOK(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (FileOpenPickerViewModel)this.DataContext;
            this.CloseDialog(new IStorageFile[] { model.SelectedFile });
        }

        private void OnCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.CloseDialog(null);
        }
    }
}