using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Edit.NET.ViewModels
{
    public partial class NewFileViewModel : ObservableValidator
    {

        [Required(AllowEmptyStrings = false)]
        [ObservableProperty]
        private string? _fileName;
    }
}
