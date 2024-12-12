using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;

namespace Example.Views
{
    public partial class DataGridTestWindow : Window
    {
        private readonly ObservableCollection<TheItem> _items;

        public DataGridTestWindow()
        {
            InitializeComponent();

            this.DataContext = new BorderViewModel();
        }
    }

    public class BorderViewModel
    {
        public BorderViewModel()
        {

            Borders = new List<ThicknessViewModel>();
            for (int i = 0; i < 16; i++)
                Borders.Add(new ThicknessViewModel(i));
        }

        public List<ThicknessViewModel> Borders { get; set; }
    }

    public class ThicknessViewModel
    {
        public ThicknessViewModel(int i)
        {
            var left = (i & 8) >> 3;
            var top = (i & 4) >> 2;
            var right = (i & 2) >> 1;
            var bottom = (i & 1);
            ThicknessText = $"{left} {top} {right} {bottom}";
            PaddingText = $"{1 - left} {1 - top} {1 - right} {1 - bottom}";
            Thickness = Thickness.Parse(ThicknessText);
            Padding = Thickness.Parse(PaddingText);
        }

        public string ThicknessText { get; set; }
        public string PaddingText { get; set; }

        public Thickness Thickness { get; set; }
        public Thickness? Padding { get; set; }
    }
}