using Avalonia.Platform.Storage;

namespace Consolonia.Core.Controls.ViewModels
{
    public class FolderPickerViewModel : PickerViewModel<FolderPickerOpenOptions>
    {
        public FolderPickerViewModel(FolderPickerOpenOptions options)
            : base(options)
        {
        }
    }
}
