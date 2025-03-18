using System;

namespace Consolonia.Core.Helpers.InputProcessor
{
    public class PasteBlockMatcher<T>(Action<string> onComplete, Func<T, char> toChar)
        : StartsEndsWithMatcher<T>(onComplete, toChar, @"[200~", @"[201~");
}