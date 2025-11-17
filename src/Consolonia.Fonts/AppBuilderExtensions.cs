using Avalonia;

namespace Consolonia.Fonts
{
    public static class AppBuilderExtensions
    {
        public static AppBuilder WithConsoleFonts(this AppBuilder appBuilder)
        {
            return appBuilder.ConfigureFonts(fontManager =>
            {
                fontManager.AddFontCollection(new ConsoleFontCollection());
            });
        }
    }
}