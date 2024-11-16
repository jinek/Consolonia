using System;
using System.Globalization;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using Consolonia.Core.Drawing;
using Consolonia.Core.Infrastructure;
using Consolonia.Gallery.View;
using Consolonia.Themes.TurboVision.Templates;

namespace Consolonia.Gallery
{
    internal class App : ConsoloniaApplication
    {
        static App()
        {
            // we want tests and UI to be executed with same culture
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        public App()
        {
            var turboVisionTheme = new TurboVisionTheme(new Uri("avares://Consolonia.Gallery"));

            if (Design.IsDesignMode)
            {
                // For previewing in Visual Studio designer without Design.PreviewWith tag we need to set default font and colors
                // get antyhing to render
                turboVisionTheme.TryGetResource("ThemeForegroundBrush", null, out var foregroundBrush);
                turboVisionTheme.TryGetResource("ThemeBackgroundBrush", null, out var backgroundBrush);
                this.Styles.Add(new Style(x => x.Is<TemplatedControl>())
                {
                    Setters =
                    {
                        // new Setter(TemplatedControl.FontSizeProperty, 16.0),
                        new Setter(TemplatedControl.FontFamilyProperty, new FontFamily("Cascadia Mono")),
                        new Setter(TemplatedControl.ForegroundProperty, new SolidColorBrush(((ConsoleBrush)foregroundBrush).Color)),
                        new Setter(TemplatedControl.BackgroundProperty, new SolidColorBrush(((ConsoleBrush)backgroundBrush).Color)),
                    }
                });
                // If you do RenderTRansform="scale(10.0,10.0) you can actually sort of see the UI get bigger
                // but this doesn' seem to work when using these style setters. <sigh>
                //this.Styles.Add(new Style(x => x.Is<Visual>())
                //{
                //    Setters =
                //    {
                //        //new Setter(Visual.RenderTransformOriginProperty, RelativePoint.TopLeft),
                //        //new Setter(Visual.RenderTransformProperty, new ScaleTransform(2.0, 2.0)),
                //        //new Setter(Visual.RenderTransformProperty, new ScaleTransform(2.0, 2.0)),
                //    }
                //});
            }

            Styles.Add(turboVisionTheme);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var lifetime = ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            if (lifetime != null)
                lifetime.MainWindow = new ControlsListView();

            base.OnFrameworkInitializationCompleted();
        }
    }
}