using System;
using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(50)]
    public partial class GalleryComboBox : UserControl
    {
        public GalleryComboBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var fontComboBox = this.Find<ComboBox>("VarsComboBox");

            fontComboBox.ItemsSource = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(environmentVariable => environmentVariable.Key + "=" + environmentVariable.Value).ToList();

            fontComboBox.SelectedIndex = 0;
        }
    }
}