using Avalonia;
using Avalonia.Interactivity;
using Consolonia;
using EditNET.DataModels;
using EditNET.ViewModels;
using Iciclecreek.Avalonia.WindowManager;
using JetBrains.Annotations;

namespace EditNET.Views
{
    public partial class EditSettingsDialog : ManagedWindow
    {
        [UsedImplicitly]
        public EditSettingsDialog()
        {
            InitializeComponent();
            if (!((ConsoloniaLifetime)Application.Current!.ApplicationLifetime!).IsRgbColorMode())
                CompatibilityErrorTxt.IsVisible = true;
        }

        public EditSettingsDialog(Settings settings) : this()
        {
            DataContext = new EditSettingsViewModel(settings);
        }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Close(((EditSettingsViewModel)DataContext!).Settings);
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}