using Consolonia;
using Consolonia.Themes;
using ConsoloniaAppTemplate.Views;

namespace ConsoloniaAppTemplate
{
    public class ConsoloniaAppTemplateApp : ConsoloniaApplication<MainWindow>
    {
        public ConsoloniaAppTemplateApp()
        {
            Styles.Add(new MaterialTheme());
        }
    }
}