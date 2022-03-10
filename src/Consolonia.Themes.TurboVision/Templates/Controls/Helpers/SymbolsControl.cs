using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Consolonia.Themes.TurboVision.Templates.Controls.Helpers
{
    public class SymbolsControl : Control
    {
        static SymbolsControl()
        {
            AffectsRender<SymbolsControl>(ForegroundProperty, TextProperty, FillProperty);
            AffectsMeasure<SymbolsControl>(TextProperty, FillProperty);
            AffectsArrange<SymbolsControl>(TextProperty, FillProperty);
        }

        /// <summary>
        /// Defines the <see cref="Foreground"/> property.
        /// </summary>
        public static readonly StyledProperty<IBrush> ForegroundProperty =
            TextBlock.ForegroundProperty.AddOwner<SymbolsControl>();

        /// <summary>
        /// Gets or sets a brush used to paint the text.
        /// </summary>
        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DirectProperty<SymbolsControl, string> TextProperty =
            TextBlock.TextProperty.AddOwnerWithDataValidation<SymbolsControl>(
                o => o.Text,
                (o, v) => o.Text = v,
                defaultBindingMode: BindingMode.TwoWay,
                enableDataValidation: true);

        private static readonly StyledProperty<bool> FillProperty =
            AvaloniaProperty.Register<SymbolsControl, bool>("Fill");

        public bool Fill
        {
            get => GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        private string _text;
        private GlyphRun _shapedText;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _shapedText = TextShaper.Current.ShapeText(
                    new ReadOnlySlice<char>((Text ?? string.Empty).ToCharArray()),
                    Typeface.Default, 1, null);
            }
        }

        public override void Render(DrawingContext context)
        {
            if (!Fill)
            {
                context.DrawGlyphRun(Foreground, _shapedText);
            }
            else
            {
                var formattedText = new FormattedText(string.Concat(Enumerable.Repeat(Text?[0] ?? ' ', (int)Bounds.Width)),
                    Typeface.Default, 1, TextAlignment.Left, TextWrapping.NoWrap, Bounds.Size);
                for (int y = 0; y < Bounds.Height; y++)
                {
                    context.DrawText(Foreground, new Point(0, y), formattedText);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return !Fill ? _shapedText?.Size ?? Size.Empty : Size.Empty;
        }
    }
}