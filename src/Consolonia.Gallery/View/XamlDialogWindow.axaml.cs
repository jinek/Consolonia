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

            var child = this.LogicalChildren.FirstOrDefault();
            if (child is InputElement input)
                input.AttachedToVisualTree += OnChildAttachedToVisualTree;
        }

        private void OnChildAttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is InputElement input)
            {
                input.AttachedToVisualTree -= OnChildAttachedToVisualTree;
                input.Focus();
            }
        }
    }
}