using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace Edit.NET
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

        private void OnNewFile_OnClick(object? sender, RoutedEventArgs e)
        {
            Editor.Text = string.Empty;
        }

        private void Exit_OnClick(object? sender, RoutedEventArgs e)
        {
            var lifetime = (IControlledApplicationLifetime)Application.Current!.ApplicationLifetime!;
            lifetime.Shutdown();
        }

        private void Cut_OnClick(object? sender, RoutedEventArgs e)
        {
            Editor.Cut();
        }
    }
}