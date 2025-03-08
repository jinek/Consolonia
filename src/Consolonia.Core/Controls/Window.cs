using System;
using Avalonia.Interactivity;
using Iciclecreek.Avalonia.WindowManager;

namespace Consolonia.Controls
{
    public class Window : ManagedWindow
    {
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            this.WindowManager.SizeChanged += WindowManager_SizeChanged;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.WindowManager.SizeChanged -= WindowManager_SizeChanged;
            base.OnClosed(e);
        }

        private void WindowManager_SizeChanged(object sender, Avalonia.Controls.SizeChangedEventArgs e)
        {
            if (WindowState == Avalonia.Controls.WindowState.Maximized)
            {
                this.Width = WindowManager.Bounds.Width;
                this.Height = WindowManager.Bounds.Height;
            }
        }
    }
}
