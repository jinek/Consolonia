using Avalonia;
using Consolonia.Core.Controls;

namespace Consolonia.Gallery.View
{
    public partial class XamlDialogWindow : DialogWindow
    {
        public XamlDialogWindow()
        {
            InitializeComponent();
            AttachedToVisualTree += OnShowDialog;
        }

        private void OnShowDialog(object sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            AttachedToVisualTree -= OnShowDialog;
            Xaml.AttachedToVisualTree += OnButtonAttached;
        }

        private void OnButtonAttached(object sender, VisualTreeAttachmentEventArgs e)
        {
            Xaml.AttachedToVisualTree -= OnButtonAttached;
            Xaml.Focus();
        }
    }
}