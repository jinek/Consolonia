using System;
using System.Text;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public class PasteBlockMatcher<T>(Action<string> onComplete, Func<T, Rune> toRune)
        : StartsEndsWithMatcher<T>(onComplete, toRune, @"[200~", @"[201~");
}