using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Example.Views
{
    public partial class DataGridView : Window
    {
        private readonly ObservableCollection<TheItem> _items;

        public DataGridView()
        {
            InitializeComponent();

            Combo.ItemsSource = TheItem.Genres;
            var rnd = new Random();

            DataContext = _items = new ObservableCollection<TheItem>(Enumerable.Range(1, 50).Select(i => new TheItem
            {
                Id = i.ToString(),
                Title = TheItem.Titles[rnd.Next(TheItem.Titles.Length)],
                Genre = TheItem.Genres[rnd.Next(TheItem.Genres.Length)],
                IsListed = i % 2 == 0
            }).ToArray());

            SetSelectedAsync();
        }

        private async void SetSelectedAsync()
        {
            await Task.Delay(100);
            var grid = this.Get<DataGrid>("Grid");
            grid.SelectedIndex = 0;
            grid.Focus();
        }


        // ReSharper disable once UnusedParameter.Local //todo: think to remove this rule
        private void Delete_Clicked(object sender, RoutedEventArgs _)
        {
            _items.Remove((TheItem)((Control)sender).DataContext);
        }
    }
}