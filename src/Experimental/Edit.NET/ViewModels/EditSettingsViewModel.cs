using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using EditNET.DataModels;
using JetBrains.Annotations;
using TextMateSharp.Grammars;

namespace EditNET.ViewModels
{
    public class EditSettingsViewModel : ObservableValidator
    {
        public EditSettingsViewModel(Settings settings)
        {
            Settings = settings;
        }

        [UsedImplicitly]
        public EditSettingsViewModel()
        {
            Settings = new Settings();
        }

        public Settings Settings { get; }

        public IReadOnlyCollection<ConsoloniaTheme> AvailableThemes { get; } = Enum.GetValues<ConsoloniaTheme>();

        public IReadOnlyCollection<ThemeName> SyntaxThemes { get; } = Enum.GetValues<ThemeName>();
    }
}