using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Iciclecreek.Avalonia.WindowManager;

// ReSharper disable once CheckNamespace
namespace Consolonia.Controls
{
    public class Window : ManagedWindow
    {
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            OverlayLayer.SizeChanged += WindowManager_SizeChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            OverlayLayer.SizeChanged -= WindowManager_SizeChanged;
            base.OnClosed(e);
        }

        private void WindowManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Width = OverlayLayer.Bounds.Width;
                Height = OverlayLayer.Bounds.Height;
            }
        }
    }
}