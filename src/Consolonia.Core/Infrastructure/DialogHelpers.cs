using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Embedding;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Consolonia.Core.Styles.Controls;
using JetBrains.Annotations;

namespace Consolonia.Core.Infrastructure
{
    public static class DialogHelpers
    {
        public static void ShowPrivate(this IControl visual, IControl parent)
        {
            /*Application.Current.ApplicationLifetime
            
            a.ApplicationLifetime;*/

            var overlayLayer = OverlayLayer.GetOverlayLayer(parent);
            var popupHost = new OverlayPopupHost(overlayLayer);
            var window = parent.FindAncestorOfType<Window>();

            popupHost.ConfigurePosition(window, PlacementMode.AnchorAndGravity,
                new Point(), PopupAnchor.TopLeft, PopupGravity.BottomRight);

            var dialogWrap = new DialogWrap();
            dialogWrap.SetContent(visual);
            popupHost.SetChild(dialogWrap);
            ContentPresenter firstContentPresenter = window.GetTemplateChildren()
                .Select(control => control.FindDescendantOfType<ContentPresenter>())
                .FirstOrDefault(d => d.Name == "PART_ContentPresenter");
            firstContentPresenter.IsEnabled = false;
            popupHost.Show();
        }
    }
}