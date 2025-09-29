using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Consolonia.Themes;
using Edit.NET.DataModels;
using TextMateSharp.Grammars;

namespace Edit.NET;

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

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            LoadUITheme(ViewModel.UITheme);
            desktopLifetime.MainWindow = new MainWindow();
            ChangeThemeVariant();
        }

        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        base.OnFrameworkInitializationCompleted();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.UITheme))
        {
            ChangeUITheme(ViewModel.UITheme);
        }
        else if (e.PropertyName == nameof(ViewModel.UIThemeVariant))
        {
             ChangeThemeVariant();
        }
    }

    private void ChangeThemeVariant()
    {
        MainWindow.RequestedThemeVariant = ViewModel.UIThemeVariant;
        EditorView.ViewModel.TextMateInstallation.SetTheme(EditorView.ViewModel.RegistryOptions.LoadTheme(MainWindow.RequestedThemeVariant == ThemeVariant.Light
            ? ThemeName.VisualStudioLight
            : ThemeName.VisualStudioDark));
        EditorView.Editor.TextArea.Focus();
    }

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
        newView.ViewModel.Syntax = EditorView.ViewModel.Syntax;
        newView.ViewModel.FilePath = EditorView.ViewModel.FilePath;
        newView.ViewModel.Modified = EditorView.ViewModel.Modified;
        newView.ViewModel.CurrentFolder = EditorView.ViewModel.CurrentFolder;

        // replace the old view with the new view
        MainWindow.Content = newView;

        newView.ViewModel.TextMateInstallation.SetTheme(newView.ViewModel.RegistryOptions.LoadTheme(MainWindow.RequestedThemeVariant == ThemeVariant.Light
                ? ThemeName.VisualStudioLight
                : ThemeName.VisualStudioDark));

        newView.Editor.TextArea.Focus();
    }

    public AppViewModel ViewModel
    {
        get => (AppViewModel)this.DataContext!;
        set => this.DataContext = value;
    }
}