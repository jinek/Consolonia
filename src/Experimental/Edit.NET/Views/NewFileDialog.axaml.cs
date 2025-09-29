using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Edit.NET.ViewModels;
using Iciclecreek.Avalonia.WindowManager;
using TextMateSharp.Grammars;

namespace Edit.NET.Views
{
    public partial class NewFileDialog : ManagedWindow
    {
        public NewFileDialog()
        {
            DataContext = new NewFileViewModel()
            {
                FileName = $"Untitled{AppViewModel.DefaultExtension}"
            };
            
            InitializeComponent();
        }

        public AppViewModel AppViewModel = (AppViewModel)App.Current.DataContext;

        private NewFileViewModel ViewModel => (NewFileViewModel)DataContext!;

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            if (ViewModel.FileName != null)
                Close(ViewModel.FileName);
            else
                Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}