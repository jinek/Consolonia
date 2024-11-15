using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PreviewHost.ViewModels;

public partial class FileViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _name = null;

    [ObservableProperty]
    private string? _fullName = null;
}


