using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.PreviewHost.ViewModels
{
    public partial class FileViewModel : ObservableObject
    {
        [ObservableProperty] private string? _fullName;

        [ObservableProperty] private string? _name;
    }
}