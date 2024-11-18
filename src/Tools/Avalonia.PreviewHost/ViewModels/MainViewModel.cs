using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.PreviewHost.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _project;

    [ObservableProperty]
    private ObservableCollection<FileViewModel> _files = new ObservableCollection<FileViewModel>();
}


