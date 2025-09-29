using Avalonia;
using Avalonia.Interactivity;
using Edit.NET.DataModels;
using Edit.NET.ViewModels;
using Iciclecreek.Avalonia.WindowManager;

namespace Edit.NET.Views
{
    public partial class EditSettingsDialog : ManagedWindow
    {
        public EditSettingsDialog(Settings settings)
        {
            InitializeComponent();

            DataContext = new EditSettingsViewModel(settings);
        }


        public EditSettingsViewModel ViewModel => (EditSettingsViewModel)DataContext!;

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Close(ViewModel.GetSettings());
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
