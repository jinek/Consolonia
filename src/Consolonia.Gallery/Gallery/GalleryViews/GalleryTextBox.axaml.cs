using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Consolonia.Gallery.Gallery.GalleryViews
{
    [GalleryOrder(80)]
    public class GalleryTextBox : UserControl
    {
        public GalleryTextBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            TextBox textBox = this.Find<TextBox>("numericWatermark");
            if (textBox != null)
                textBox
                    .TextInputOptionsQuery += (_, a) =>
                {
                    a.ContentType = Avalonia.Input.TextInput.TextInputContentType.Number;
                };
        }
    }
}