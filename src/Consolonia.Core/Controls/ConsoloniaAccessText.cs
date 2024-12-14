using System;
using Avalonia.Media;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Documents;

namespace Consolonia.Core.Controls
{

    /// <summary>
    /// A text block that displays a character prefixed with an underscore as an access key.
    /// </summary>
    public sealed class ConsoloniaAccessText : AccessText
    {
        private Run _accessRun;

        public ConsoloniaAccessText()
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            switch (e.Property.Name)
            {
                case nameof(Text):
                    {
                        if (!String.IsNullOrEmpty(Text))
                        {
                            _accessRun = null;
                            InlineCollection inlines = new InlineCollection();
                            var iPos = Text.IndexOf('_', StringComparison.Ordinal);
                            if (iPos >= 0 && iPos < Text.Length - 1)  
                            {
                                inlines.Add(new Run(Text.Substring(0, iPos)));
                                _accessRun = new Run(Text.Substring(++iPos, 1))
                                {
                                    TextDecorations = new TextDecorationCollection()
                                };
                                inlines.Add(_accessRun);
                                inlines.Add(new Run(Text.Substring(iPos + 1)));
                            }
                            else
                            {
                                inlines.Add(new Run(Text));
                            }
                            this.Inlines = inlines;
                        }
                    }
                    break;

                case nameof(ShowAccessKey):
                    if (_accessRun != null)
                    {
                        if (this.ShowAccessKey)
                            _accessRun.TextDecorations.Add(new TextDecoration { Location = TextDecorationLocation.Underline });
                        else
                            _accessRun.TextDecorations.Clear();
                    }
                    break;
            }
        }
    }
}