using System;
using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;

namespace Consolonia.Controls
{
    /// <summary>
    ///     A text block that displays a character prefixed with an underscore as an access key.
    /// </summary>
    public sealed class ConsoloniaAccessText : AccessText
    {
        private Run _accessRun;

        static ConsoloniaAccessText()
        {
            AffectsRender<ConsoloniaAccessText>(ShowAccessKeyProperty);
        }

        public ConsoloniaAccessText()
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case nameof(Text):
                {
                    if (!string.IsNullOrEmpty(Text))
                    {
                        var inlines = new InlineCollection();
                        int iPos = Text.IndexOf('_', StringComparison.Ordinal);
                        if (iPos >= 0 && iPos < Text.Length - 1)
                        {
                            inlines.Add(new Run(Text[..iPos]));
                            _accessRun = new Run(Text[++iPos..++iPos]);
                            inlines.Add(_accessRun);
                            inlines.Add(new Run(Text[iPos..]));
                        }
                        else
                        {
                            _accessRun = null;
                            inlines.Add(new Run(Text));
                        }

                        Inlines = inlines;
                    }
                }
                    break;

                case nameof(ShowAccessKey):
                    if (_accessRun != null)
                    {
                        if (ShowAccessKey)
                            _accessRun.TextDecorations = Avalonia.Media.TextDecorations.Underline;
                        else
                            _accessRun.TextDecorations = null;
                    }

                    break;
            }
        }
    }
}