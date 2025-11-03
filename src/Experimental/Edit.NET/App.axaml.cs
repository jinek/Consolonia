using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Consolonia.Themes;
using EditNET.ViewModels;
using EditNET.Views;

namespace EditNET;

public partial class App : Application
{

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Load settings
        this.DataContext = AppViewModel.LoadSettings();
    }

    public MainWindow MainWindow => (MainWindow)((IClassicDesktopStyleApplicationLifetime)this.ApplicationLifetime!).MainWindow!;

    public EditorView EditorView => (EditorView)this.MainWindow.Content!;

    public static AppViewModel ViewModel => (AppViewModel)App.Current!.DataContext!;

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            LoadUITheme(ViewModel.UITheme);
            desktopLifetime.MainWindow = new MainWindow();
            if (desktopLifetime.Args != null && desktopLifetime.Args.Length > 0)
            {
                // Open the file passed as argument
                await EditorView.ViewModel.OpenFile(desktopLifetime.Args[0]);
            }
            MainWindow.RequestedThemeVariant = ViewModel.UIThemeVariant;
        }

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        base.OnFrameworkInitializationCompleted();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppViewModel.UITheme))
        {
            ChangeUITheme(ViewModel.UITheme);
        }
        else if (e.PropertyName == nameof(AppViewModel.UIThemeVariant))
        {
            MainWindow.RequestedThemeVariant = ViewModel.UIThemeVariant;
        }
        else if (e.PropertyName == nameof(AppViewModel.SyntaxTheme))
        {
            var theme = EditorView.ViewModel!.RegistryOptions!.LoadTheme(ViewModel.SyntaxTheme);
            EditorView.ViewModel.TextMateInstallation!.SetTheme(theme);
        }
    }


    //public ThemeName GetSyntaxTheme()
    //{
    //    if (MainWindow.RequestedThemeVariant == ThemeVariant.Light)
    //        return GetSyntaxThemeLight();
    //    if (MainWindow.RequestedThemeVariant == ThemeVariant.Dark)
    //        return GetSyntaxThemeDark();
    //    return ViewModel.SyntaxTheme;
    //}

    //private ThemeName GetSyntaxThemeLight()
    //{
    //    if (ViewModel.SyntaxTheme.ToString().EndsWith("Light"))
    //        return ViewModel.SyntaxTheme;
    //    else if (ViewModel.SyntaxTheme.ToString().EndsWith("Dark") && Enum.TryParse<ThemeName>(ViewModel.SyntaxTheme.ToString().Replace("Dark", "Light"), out var theme))
    //        return theme;
    //    else
    //        return ViewModel.SyntaxTheme;
    //}

    //private ThemeName GetSyntaxThemeDark()
    //{
    //    if (ViewModel.SyntaxTheme.ToString().EndsWith("Dark"))
    //        return ViewModel.SyntaxTheme;
    //    else if (ViewModel.SyntaxTheme.ToString().EndsWith("Light") && Enum.TryParse<ThemeName>(ViewModel.SyntaxTheme.ToString().Replace("Light", "Dark"), out var theme))
    //        return theme;
    //    else
    //        return ViewModel.SyntaxTheme;
    //}

    private static void LoadUITheme(ConsoloniaTheme theme)
    {
        // NOTE: this assumes first style object is the old theme!
        Application.Current!.Styles[0] = theme switch
        {
            ConsoloniaTheme.Modern => new ModernTheme(),
            ConsoloniaTheme.ModernContrast => new ModernContrastTheme(),
            ConsoloniaTheme.TurboVision => new TurboVisionTheme(),
            ConsoloniaTheme.TurboVisionCompatible => new TurboVisionCompatibleTheme(),
            ConsoloniaTheme.TurboVisionGray => new TurboVisionGrayTheme(),
            ConsoloniaTheme.TurboVisionElegant => new TurboVisionElegantTheme(),
            _ => throw new InvalidDataException("Unknown theme name")
        };
    }

    private void ChangeUITheme(ConsoloniaTheme theme)
    {
        LoadUITheme(theme);

        // Recreate the editor view to ensure proper theme application
        var newView = new EditorView();

        // the newView has it's own ViewModel created in the constructor bound to it's own editor,
        // so we copy over the relevant information.
        newView.Editor.Text = EditorView.Editor.Text;
        newView.ViewModel.FilePath = EditorView.ViewModel.FilePath;
        newView.ViewModel.Modified = EditorView.ViewModel.Modified;
        newView.ViewModel.CurrentFolder = EditorView.ViewModel.CurrentFolder;

        // replace the old view with the new view
        MainWindow.Content = newView;

        newView.ViewModel.TextMateInstallation!.SetTheme(newView.ViewModel.RegistryOptions!.LoadTheme(ViewModel.SyntaxTheme));

        newView.Editor.TextArea.Focus();
    }

}