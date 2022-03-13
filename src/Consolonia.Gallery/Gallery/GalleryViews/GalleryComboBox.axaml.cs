using System;
using System.Collections;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [UsedImplicitly]
    [GalleryOrder(50)]
    public class GalleryComboBox : UserControl
    {
        public GalleryComboBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var fontComboBox = this.Find<ComboBox>("VarsComboBox");

            fontComboBox.Items = Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .Select(environmentVariable => environmentVariable.Key + "=" + environmentVariable.Value).ToList();

            fontComboBox.SelectedIndex = 0;
        }
    }
}