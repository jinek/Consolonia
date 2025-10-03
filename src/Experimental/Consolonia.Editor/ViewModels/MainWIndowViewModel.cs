using System.Collections.ObjectModel;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using AvaloniaEdit.TextMate;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TextMateSharp.Grammars;

namespace ConsoloniaEdit.Demo.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly TextMate.Installation _textMateInstallation;
    private readonly RegistryOptions _registryOptions;

    [ObservableProperty]
    private ObservableCollection<ThemeViewModel> _allThemes = new();

    [ObservableProperty]
    private ThemeViewModel _selectedTheme;

    public MainWindowViewModel(TextMate.Installation textMateInstallation, RegistryOptions registryOptions)
    {
        PropertyChanged += MainWindowViewModel_PropertyChanged;
        _textMateInstallation = textMateInstallation;
        _registryOptions = registryOptions;
    }

    private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedTheme))
        {
            _textMateInstallation.SetTheme(_registryOptions.LoadTheme(SelectedTheme.ThemeName));
        }
    }

    [RelayCommand]
    public void CopyMouse(TextArea textArea)
    {
        ApplicationCommands.Copy.Execute(null, textArea);
    }

    [RelayCommand]
    public void CutMouse(TextArea textArea)
    {
        ApplicationCommands.Cut.Execute(null, textArea);
    }
    
    [RelayCommand]
    public void PasteMouse(TextArea textArea)
    {
        ApplicationCommands.Paste.Execute(null, textArea);
    }

    [RelayCommand]
    public void SelectAllMouse(TextArea textArea)
    {
        ApplicationCommands.SelectAll.Execute(null, textArea);
    }

    // Undo Status is not given back to disable it's item in ContextFlyout; therefore it's not being used yet.
    [RelayCommand]
    public void UndoMouse(TextArea textArea)
    {
        ApplicationCommands.Undo.Execute(null, textArea);
    }
}