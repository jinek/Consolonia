using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using Consolonia.Core.Drawing;

namespace Example.Views
{
    public partial class DataGridTestWindow : Window
    {
        private readonly ObservableCollection<TheItem> _items;

        public DataGridTestWindow()
        {
            InitializeComponent();
        }


    }

}