using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Consolonia.Core.Controls.Dialog;
using Consolonia.Core.Controls.ViewModels;
using Consolonia.Core.Infrastructure;

namespace Consolonia.Core.Controls.Views
{
    public partial class FileOpenPicker : DialogWindow
    {
        public FileOpenPicker(ushort width, ushort height, FilePickerOpenOptions options)
        {
            Width = width;
            Height = height;
            Loaded += (_, _) =>
            {
                this.FindControl<Button>("Button")?.Focus();
            };
            DataContext = new FileOpenPickerViewModel(options);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public FilePickerOpenOptions Options => ((FileOpenPickerViewModel)DataContext).Options;

        private void OnDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (e.Source is SystemStorageFolder folder)
            {
                var model = (FileOpenPickerViewModel)this.DataContext;
                model.CurrentFolder = folder;
            }
            if (e.Source is SystemStorageFile file)
            {
                var model = (FileOpenPickerViewModel)this.DataContext;
                model.SelectedItem = file;
            }
        }

        private void OnOK(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var model = (FileOpenPickerViewModel)this.DataContext;
            this.CloseDialog(new IStorageFile[] { (IStorageFile)model.SelectedItem });
        }

        private void OnCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            this.CloseDialog(null);
        }
    }
}