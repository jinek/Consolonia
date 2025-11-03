using Avalonia.Interactivity;
using EditNET.ViewModels;
using Iciclecreek.Avalonia.WindowManager;

namespace EditNET.Views
{
    public partial class NewFileDialog : ManagedWindow
    {
        public NewFileDialog()
        {
            DataContext = new NewFileViewModel()
            {
                FileName = $"Untitled{App.ViewModel.DefaultExtension}"
            };
            
            InitializeComponent();
        }

        private NewFileViewModel ViewModel => (NewFileViewModel)DataContext!;

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (ViewModel.FileName != null)
                Close(ViewModel.FileName);
            else
                Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}