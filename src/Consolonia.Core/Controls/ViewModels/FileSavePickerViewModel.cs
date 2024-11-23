using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls.ViewModels
{
    public class FileSavePickerViewModel : PickerViewModel<FilePickerSaveOptions>
    {
        public FileSavePickerViewModel(FilePickerSaveOptions options)
            : base(options)
        {
        }
    }
}
