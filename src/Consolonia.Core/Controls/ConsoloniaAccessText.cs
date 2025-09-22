using System;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace Consolonia.Core.Controls
{
    /// <summary>
    ///     A text block that displays a character prefixed with an underscore as an access key.
    /// </summary>
    public sealed class ConsoloniaAccessText : AccessText
    {
        private Run _accessRun;
        private IBrush _realForeground;

        static ConsoloniaAccessText()
        {
            AffectsRender<ConsoloniaAccessText>(AccessText.ShowAccessKeyProperty);
        }

        public ConsoloniaAccessText()
        {
            PropertyChanged += OnPropertyChanged;
        }

        protected override TextLayout CreateTextLayout(string text)
        {
            Foreground = Brushes.Transparent;
            return base.CreateTextLayout(text);
        }

        private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case nameof(Foreground):
                    {
                        // this bit jackassery is because Avalonia doesn't let me
                        // override the render logic of AccessText to not draw a line
                        // It also doesn't give me access to IAccessKeyManager
                        // So instead I just make the foreground transparent so that
                        // AccessText draws a transparent line, and then when I 
                        // create the TextRuns I use the "real" foreground.
                        // This can go away if Avalonia makes one of these changes:
                        // * AccessText lets me override RenderCore
                        // * AccessText lets me get access to IAccessKeyManager
                        // * AccessText lets me disable the underline
                        // * AccessText lets control the line thickness 
                        // another way of solving this would be to create a custom brush
                        // of some sort, but this feels less intrusive.
                        var brush = e.NewValue as IBrush;
                        if (brush != Brushes.Transparent)
                        {
                            _realForeground = brush;
                            Foreground = Brushes.Transparent;
                        }
                    }
                    break;
                case nameof(Text):
                    {
                        if (!string.IsNullOrEmpty(Text))
                        {
                            var inlines = new InlineCollection();
                            int iPos = Text.IndexOf('_', StringComparison.Ordinal);
                            if (iPos >= 0 && iPos < Text.Length - 1)
                            {
                                inlines.Add(new Run(Text[..iPos]) { Foreground = _realForeground ?? this.Foreground });
                                _accessRun = new ConsoloniaAccessRun(Text[++iPos..++iPos]) { Foreground = _realForeground ?? this.Foreground };
                                inlines.Add(_accessRun);
                                inlines.Add(new Run(Text[iPos..]) { Foreground = _realForeground ?? this.Foreground });
                            }
                            else
                            {
                                _accessRun = null;
                                inlines.Add(new Run(Text) { Foreground = _realForeground ?? this.Foreground });
                            }

                            Inlines = inlines;
                        }
                    }
                    break;
            }
        }
    }
}