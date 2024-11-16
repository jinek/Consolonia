using Avalonia.Controls;
using Consolonia.PreviewHost.ViewModels;

namespace Consolonia.PreviewHost.Views;

public partial class HeadlessWindow : Window
{
    public HeadlessWindow()
    {
        InitializeComponent();
    }

    public ProjectViewModel Model => (ProjectViewModel)DataContext!;

    private void OnKeyUp(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        this.Close();
    }
}

