using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace ConsoloniaAppTemplate
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            var lifetime = Application.Current!.ApplicationLifetime as IControlledApplicationLifetime;
            lifetime!.Shutdown();
        }
    }
}