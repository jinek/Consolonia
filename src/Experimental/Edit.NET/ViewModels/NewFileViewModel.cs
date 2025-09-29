using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TextMateSharp.Grammars;

namespace Edit.NET.ViewModels
{
    public partial class NewFileViewModel : ObservableValidator
    {

        [Required(AllowEmptyStrings = false)]
        [ObservableProperty]
        private string _fileName;
    }
}
