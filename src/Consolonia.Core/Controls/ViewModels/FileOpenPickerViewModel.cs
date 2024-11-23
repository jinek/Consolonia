using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls.ViewModels
{
    public class FileOpenPickerViewModel : PickerViewModel<FilePickerOpenOptions>
    {
        public FileOpenPickerViewModel(FilePickerOpenOptions options)
            : base(options)
        {
        }
    }
}
