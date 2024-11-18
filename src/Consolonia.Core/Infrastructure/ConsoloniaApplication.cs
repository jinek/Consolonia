using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Consolonia.Core.Drawing;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaApplication : Application
    {
        public override void RegisterServices()
        {
            base.RegisterServices();
            var keyboardNavigationHandler = AvaloniaLocator.Current.GetRequiredService<IKeyboardNavigationHandler>();
            AvaloniaLocator.CurrentMutable.Bind<IKeyboardNavigationHandler>()
                .ToConstant(new ArrowsAndKeyboardNavigationHandler(keyboardNavigationHandler));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            base.OnFrameworkInitializationCompleted();

            if (Design.IsDesignMode)
            {
                // For previewing in Visual Studio designer without Design.PreviewWith tag we need to set default font and colors
                // get anything to render. This is not perfect, but nicer than getting a big error screen.
                IBrush foregroundBrush = Brushes.White;
                if (this.Styles.TryGetResource("ThemeForegroundBrush", null, out var brush))
                    foregroundBrush = (IBrush)brush;

                IBrush backgroundBrush = Brushes.Black;
                if (!this.Styles.TryGetResource("ThemeBackgroundBrush", null, out brush))
                    backgroundBrush = (IBrush)brush;

                this.Styles.Add(new Style(x => x.Is<TemplatedControl>())
                {
                    Setters =
                    {
                        new Setter(TemplatedControl.FontSizeProperty, 16.0),
                        new Setter(TemplatedControl.FontFamilyProperty, new FontFamily("Cascadia Mono")),
                        new Setter(TemplatedControl.ForegroundProperty, foregroundBrush),
                        new Setter(TemplatedControl.BackgroundProperty, backgroundBrush),
                    }
                });

                // EXPERIMENTAL
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


        }
    }
}