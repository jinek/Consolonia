using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Consolonia.Core.Controls.ViewModels;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls.Views
{
    public partial class FileOpenPicker : Window
    {
        public FileOpenPicker(ushort width, ushort height)
        {
            InitializeComponent();
            Width = width;
            Height = height;
            Loaded += (_, _) =>
            {
                this.FindControl<Button>("Button")!.Focus();
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }



        private void OnDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (e.Source is StorageFolder folder)
            {
                var model  = (FileOpenPickerViewModel)this.DataContext;
                model.CurrentFolder = folder;
            }
            if (e.Source is StorageFile file)
            {
                var model = (FileOpenPickerViewModel)this.DataContext;
                model.SelectedItem = file;
            }
        }

    }
}