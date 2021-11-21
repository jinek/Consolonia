using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;
using VisualExtensions = Avalonia.VisualTree.VisualExtensions;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Dialog
{
    public static class DialogHelpers
    {
        public static void ShowDialogPrivate(this Control visual, IControl parent)
        {
            DialogHost dialogHost = GetDialogHost(parent);
            dialogHost.OpenInternal(visual);
        }

        public static void CloseDialog(this Control visual)
        {
            DialogHost dialogHost = GetDialogHost(visual);
            dialogHost.PopInternal(visual);
        }

        private static DialogHost GetDialogHost(IControl parent)
        {
            var window = parent.FindAncestorOfType<Window>();
            DialogHost dialogHost = window.GetValue(DialogHost.DialogHostProperty);
            return dialogHost;
        }
    }

    public class DialogHost
    {
        public static readonly AttachedProperty<bool> IsDialogHostProperty =
            AvaloniaProperty.RegisterAttached<Button, bool>("IsDialogHost", typeof(DialogHost));

        internal static readonly AttachedProperty<DialogHost> DialogHostProperty =
            AvaloniaProperty.RegisterAttached<Button, DialogHost>("DialogHost", typeof(DialogHost));

        private readonly Window _window;

        static DialogHost()
        {
            IsDialogHostProperty.Changed.Subscribe(args =>
            {
                if (args.NewValue.Value)
                {
                    args.Sender.SetValue(DialogHostProperty, new DialogHost((Window)args.Sender));
                }
                else
                {
                    throw new NotImplementedException();
                }
            });
        }

        private DialogHost(Window window)
        {
            _window = window;
        }

        private readonly Stack<OverlayPopupHost> _dialogs = new();

        public void OpenInternal(Control visual)
        {
            var overlayLayer = OverlayLayer.GetOverlayLayer(_window);
            var popupHost = new OverlayPopupHost(overlayLayer);

            popupHost.ConfigurePosition(_window, PlacementMode.AnchorAndGravity,
                new Point(), PopupAnchor.TopLeft, PopupGravity.BottomRight);

            var dialogWrap = new DialogWrap();
            dialogWrap.SetContent(visual);
            popupHost.SetChild(dialogWrap);
            GetFirstContentPresenter().IsEnabled = false;

            if (_dialogs.TryPeek(out OverlayPopupHost previousDialog))
            {
                previousDialog.IsEnabled = false;
            }

            _dialogs.Push(popupHost);
            popupHost.Show();
        }

        private ContentPresenter GetFirstContentPresenter()
        {
            ContentPresenter firstContentPresenter = _window.GetTemplateChildren()
                .Select(control => VisualExtensions.FindDescendantOfType<ContentPresenter>(control))
                .First(d => d.Name == "PART_ContentPresenter");
            return firstContentPresenter;
        }

        public void PopInternal(Control control)
        {
            OverlayPopupHost overlayPopupHost = _dialogs.Pop();
            if (((DialogWrap)overlayPopupHost.Content).Content != control)
                throw new InvalidOperationException("Dialog is not topmost. Close private dialogs first");
            overlayPopupHost.Hide();
            if (_dialogs.TryPeek(out OverlayPopupHost previousDialog))
            {
                previousDialog.IsEnabled = true;
                previousDialog.Focus();
            }

            if (_dialogs.Count == 0)
            {
                ContentPresenter firstContentPresenter = GetFirstContentPresenter();
                firstContentPresenter.IsEnabled = true;
                firstContentPresenter.Focus();
            }
        }
    }
}