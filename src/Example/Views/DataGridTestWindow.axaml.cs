using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Example.Views
{
    public class DataGridTestWindow : Window
    {
        private readonly ObservableCollection<TheItem> _items;

        public DataGridTestWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var comboBox = this.FindControl<ComboBox>("Combo");
            comboBox.Items = TheItem.Genres;
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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // ReSharper disable once UnusedParameter.Local //todo: think to remove this rule
        private void delete_Clicked(object sender, RoutedEventArgs _)
        {
            _items.Remove((TheItem)((Control)sender).DataContext);
        }
    }
}