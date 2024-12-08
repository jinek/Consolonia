using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.PreviewHost.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<FileViewModel> _files = new();

        [ObservableProperty] private string? _project;
    }
}