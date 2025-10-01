using System;
using Avalonia;
using Avalonia.Media;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using Consolonia.Controls;
using Consolonia.Controls.Brushes;

namespace Consolonia.AvaloniaEdit
{
    /// <summary>
    ///     This attached property is used to enable console style caret in AvaloniaEdit TextEditor.
    /// </summary>
    /// <remarks>
    ///     It is autoamtically added using the Consolonia.AvaloniaEdit Theme setter
    ///     It can be explitily added to a text editor XAML using: console:Caret.UseConsole="True"
    ///     It can be explicitly added to a text editor in code using: Caret.SetUseConsole(textEditor, true);
    /// </remarks>
    public sealed class Caret
    {
        public static readonly AttachedProperty<bool> UseConsoleProperty =
            AvaloniaProperty.RegisterAttached<Caret, TextEditor, bool>("UseConsole");

        static Caret()
        {
            IBrush oldBrush = null;
            UseConsoleProperty.Changed.AddClassHandler<TextEditor>((textEditor, e) =>
            {
                bool value = (bool)e.NewValue;
                if (value)
                {
                    // replace caret with console caret.
                    oldBrush = textEditor.TextArea.Caret.CaretBrush;
#if USE_CONSOLE_CARET
                    textEditor.TextArea.Caret.CaretBrush = new MoveConsoleCaretToPositionBrush
                    { CaretStyle = CaretStyle.SteadyBar };
#endif
                    textEditor.TextArea.PropertyChanged += TextArea_PropertyChanged;

                    // The built in LineNumberMargin miscalculates the top of the line, 
                    // we substitute ours with one which works correctly.
                    for (int i = 0; i < textEditor.TextArea.LeftMargins.Count; i++)
                    {
                        if (textEditor.TextArea.LeftMargins[i] is LineNumberMargin)
                        {
                            textEditor.TextArea.LeftMargins[i] = new ConsoleLineNumberMargin();
                            break;
                        }
                    }
                }
                else
                {
                    // restore default caret
                    textEditor.TextArea.Caret.CaretBrush = oldBrush;
                    textEditor.TextArea.PropertyChanged -= TextArea_PropertyChanged;
                }
            });
        }

        public static void SetUseConsole(TextEditor textEditor, bool value)
        {
            textEditor.SetValue(UseConsoleProperty, value);
        }

        public static bool GetUseConsole(TextEditor textEditor)
        {
            return textEditor.GetValue(UseConsoleProperty);
        }

        private static void TextArea_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            // monitor OverstrikeMode property changes to update caret style to match
            if (e.Property == TextArea.OverstrikeModeProperty)
            {
#if USE_CONSOLE_CARET
                var textArea = (TextArea)sender;
                if (textArea.Caret.CaretBrush is MoveConsoleCaretToPositionBrush caretBrush)
                    // NOTE: We use SteadyBlock and SteadyBar because AvaloniaEdit has blinking animation hardcoded in.
                    caretBrush.CaretStyle = (bool)e.NewValue
                        ? CaretStyle.SteadyBlock
                        : CaretStyle.SteadyBar;
#endif
            }
        }
    }
}