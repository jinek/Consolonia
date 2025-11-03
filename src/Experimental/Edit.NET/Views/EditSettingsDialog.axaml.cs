using Avalonia.Interactivity;
using EditNET.DataModels;
using EditNET.ViewModels;
using Iciclecreek.Avalonia.WindowManager;

namespace EditNET.Views
{
    public partial class EditSettingsDialog : ManagedWindow
    {
        public EditSettingsDialog()
        {
        }

        public EditSettingsDialog(Settings settings)
        {
            InitializeComponent();

            DataContext = new EditSettingsViewModel(settings);
        }


        private EditSettingsViewModel ViewModel => (EditSettingsViewModel)DataContext!;

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Close(ViewModel.ToSettings());
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close(null);
        }
    }
}
