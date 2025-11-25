using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Sandbox
{
    public partial class MainWindow : Window
    {
        public int[] sequences = [0, 1, 2, 3];
        private int _iteration = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnExit(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(sender);
            ArgumentNullException.ThrowIfNull(e);

            var lifetime = Application.Current!.ApplicationLifetime as IControlledApplicationLifetime;
            lifetime!.Shutdown();
        }

        private void InputElement_OnKeyDown(object? sender, KeyEventArgs e)
        {
            overlapTxt.Margin = new Thickness(sequences[_iteration++ % sequences.Length], 0, 0, 0);
        }
    }
}