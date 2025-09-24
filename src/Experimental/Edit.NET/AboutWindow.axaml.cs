using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Iciclecreek.Avalonia.WindowManager;

namespace Edit.NET;

public partial class AboutWindow : ManagedWindow
{
    public AboutWindow()
    {
        InitializeComponent();
        TrySetVersion();
    }

    private void TrySetVersion()
    {
        var asm = Assembly.GetExecutingAssembly();
        string ver = asm.GetName().Version?.ToString() ?? "";
        VersionText.Text = string.IsNullOrWhiteSpace(ver) ? "" : $"Version {ver}";
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}