using System;

namespace Consolonia.Core.Helpers.InputProcessing
{
    public class PasteBlockMatcher<T>(Action<string> onComplete, Func<T, string> toText)
        : StartsEndsWithMatcher<T>(onComplete, toText, @"[200~", @"[201~");
}