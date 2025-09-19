using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Edit.NET;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        TrySetVersion();
    }

    private void TrySetVersion()
    {
        try
        {
            var asm = Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version?.ToString() ?? "";
            VersionText.Text = string.IsNullOrWhiteSpace(ver) ? "" : $"Version {ver}";
        }
        catch
        {
            // ignore
        }
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}