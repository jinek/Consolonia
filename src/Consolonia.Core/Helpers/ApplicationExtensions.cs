using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;
using Consolonia.Core.Controls;

// ReSharper disable CheckNamespace
namespace Consolonia
{
    public static class ApplicationExtensions
    {
        public static void InitializeConsolonia(this Application application)
        {
            // override AccessText to use ConsoloniaAccessText as default ContentPresenter for unknown data types (aka string)
            application.DataTemplates.Add(new FuncDataTemplate<object>(
                (data, _) =>
                {
                    if (data != null)
                    {
                        var result = new ConsoloniaAccessText();
                        // ReSharper disable AccessToStaticMemberViaDerivedType
                        result.Bind(TextBlock.TextProperty,
                            result.GetBindingObservable(Control.DataContextProperty, x => x?.ToString()));
                        return result;
                    }

                    return null;
                },
                true)
            );


            if (Design.IsDesignMode)
            {
                // For previewing in Visual Studio designer without Design.PreviewWith tag we need to set default font and colors
                // get anything to render. This is not perfect, but nicer than getting a big error screen.
                IBrush foregroundBrush = Brushes.White;
                if (application.Styles.TryGetResource("ThemeForegroundBrush", null, out object brush))
                    foregroundBrush = (IBrush)brush;

                IBrush backgroundBrush = Brushes.Black;
                if (application.Styles.TryGetResource("ThemeBackgroundBrush", null, out brush))
                    backgroundBrush = (IBrush)brush;

                application.Styles.Add(new Style(x => x.Is<TemplatedControl>())
                {
                    Setters =
                    {
                        new Setter(TemplatedControl.FontSizeProperty, 16.0),
                        new Setter(TemplatedControl.FontFamilyProperty, new FontFamily("Cascadia Mono")),
                        new Setter(TemplatedControl.ForegroundProperty, foregroundBrush),
                        new Setter(TemplatedControl.BackgroundProperty, backgroundBrush)
                    }
                });

                // EXPERIMENTAL
                // If you do RenderTransform="scale(10.0,10.0) you can actually sort of see the UI get bigger
                // but this doesn't seem to work when using these style setters. <sigh>
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