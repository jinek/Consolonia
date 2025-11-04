using System.ComponentModel.DataAnnotations;
using TextMateSharp.Grammars;

namespace EditNET.DataModels
{
    public class Settings
    {
        public ConsoloniaTheme ConsoloniaTheme { get; set; } = ConsoloniaTheme.Modern;

        public bool LightVariant { get; set; }

        public bool ShowTabs { get; set; }

        public bool ShowSpaces { get; set; }

        [RegularExpression(@"^(?i)\.[a-z0-9]+$", ErrorMessage = "Invalid extension")]
        public string DefaultExtension { get; set; } = ".txt";

        public ThemeName SyntaxTheme { get; set; } = ThemeName.VisualStudioDark;
    }
}