using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Utilities;

namespace Consolonia.Core.Styles.Controls.Helpers
{
    public class SymbolsControl : Control
    {
        static SymbolsControl()
        {
            AffectsRender<SymbolsControl>(ForegroundProperty, TextProperty);
            AffectsMeasure<SymbolsControl>(TextProperty);
            AffectsArrange<SymbolsControl>(TextProperty);
        }

        /// <summary>
        /// Defines the <see cref="Foreground"/> property.
        /// </summary>
        public static readonly AttachedProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.RegisterAttached<SymbolsControl, Control, IBrush>(
                nameof(Foreground),
                Brushes.Black,
                true);

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

        private string _text;
        private GlyphRun _shapedText;

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _shapedText = TextShaper.Current.ShapeText(new ReadOnlySlice<char>(Text.ToCharArray()),
                    Typeface.Default, 1, null);
            }
        }

        public override void Render(DrawingContext context)
        {
            context.DrawGlyphRun(Foreground, _shapedText);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return _shapedText?.Size??Size.Empty;
        }
    }
}