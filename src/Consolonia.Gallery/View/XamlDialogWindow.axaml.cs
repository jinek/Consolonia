using System.Linq;
using Avalonia;
using Avalonia.Input;
using Consolonia.Core.Controls;

namespace Consolonia.Gallery.View
{
    public partial class XamlDialogWindow : DialogWindow
    {
        public XamlDialogWindow()
        {
            InitializeComponent();

            AttachedToVisualTree += DialogWindowAttachedToVisualTree;
        }

        private void DialogWindowAttachedToVisualTree(object sender, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            AttachedToVisualTree -= DialogWindowAttachedToVisualTree;

            var child = (InputElement)this.LogicalChildren.FirstOrDefault();
            if (child != null)
                child.AttachedToVisualTree += OnChildAttachedToVisualTree;
        }

        private void OnChildAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            var child = (InputElement)sender;
            child.AttachedToVisualTree -= OnChildAttachedToVisualTree;
            // Set focus to the first focusable element
            child?.Focus();
        }
    }
}