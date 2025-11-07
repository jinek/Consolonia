using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using EditNET.DataModels;
using JetBrains.Annotations;
using TextMateSharp.Grammars;

namespace EditNET.ViewModels
{
    public partial class EditSettingsViewModel : ObservableValidator
    {
        public EditSettingsViewModel(Settings settings)
        {
            Settings = settings;
        }

        public Settings Settings { get; }

        public IReadOnlyCollection<ConsoloniaTheme> AvailableThemes { get; } = Enum.GetValues<ConsoloniaTheme>();

        public IReadOnlyCollection<ThemeName> SyntaxThemes { get; } = Enum.GetValues<ThemeName>();

        [UsedImplicitly]
        public EditSettingsViewModel()
        {
            Settings = new Settings();
        }
    }
}