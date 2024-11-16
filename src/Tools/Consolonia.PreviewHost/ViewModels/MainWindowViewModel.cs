using CommunityToolkit.Mvvm.ComponentModel;

namespace Consolonia.PreviewHost.ViewModels
{
    public partial class AppViewModel : ObservableObject
    {

        [ObservableProperty]
        private ProjectViewModel _project = null;
    }
}
