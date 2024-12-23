using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ConsoloniaAppTemplate.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void OnExit(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}