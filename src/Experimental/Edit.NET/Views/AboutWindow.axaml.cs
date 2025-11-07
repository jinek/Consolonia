using System.Reflection;
using Avalonia.Interactivity;
using Iciclecreek.Avalonia.WindowManager;

namespace EditNET.Views
{
    public partial class AboutWindow : ManagedWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
            SetVersion();
        }

        private void SetVersion()
        {
            var asm = Assembly.GetExecutingAssembly();
            string ver = asm.GetName().Version!.ToString();
            VersionText.Text = $"Version {ver}";
        }

        private void Ok_OnClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}