using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PreviewHost.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string? project;

    [ObservableProperty]
    private ObservableCollection<FileViewModel> files = new ObservableCollection<FileViewModel>();
}


