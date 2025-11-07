using AvaloniaEdit;

namespace Consolonia.AvaloniaEdit
{
    public static class TextEditorExtensions
    {
        /// <summary>
        ///  Enable Consolonia features in the TextEditor
        /// </summary>
        /// <param name="textEditor"></param>
        public static void UseConsolonia(this TextEditor textEditor)
        {
            Caret.SetUseConsole(textEditor, true);

            // add fontmetric transformer so that underlines and strikethroughs use glyph metrics
            textEditor.TextArea.TextView.LineTransformers.Add(new FontMetricsTransformer());
        }
    }
}
