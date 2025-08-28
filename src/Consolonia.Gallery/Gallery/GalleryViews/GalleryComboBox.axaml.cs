using System;
using System.Collections;
using System.Linq;
using Avalonia.Controls;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(50)]
    public partial class GalleryComboBox : UserControl
    {
        public GalleryComboBox()
        {
            InitializeComponent();

            VarsComboBox.ItemsSource = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(environmentVariable => environmentVariable.Key + "=" + environmentVariable.Value).ToList();

            VarsComboBox.SelectedIndex = 0;
        }
    }
}