using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PreviewHost.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? project;

    [ObservableProperty]
    private ObservableCollection<FileViewModel> files = new ObservableCollection<FileViewModel>();
}


