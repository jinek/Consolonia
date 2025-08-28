using System;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public class PasteBlockMatcher<T>(Action<string> onComplete, Func<T, char> toChar)
        : StartsEndsWithMatcher<T>(onComplete, toChar, @"[200~", @"[201~");
}