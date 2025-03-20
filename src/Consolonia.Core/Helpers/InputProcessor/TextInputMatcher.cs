using System;

namespace Consolonia.Core.Helpers.InputProcessor
{
    /// <summary>
    ///     A matcher that matches any text input.
    /// </summary>
    public class TextInputMatcher<T>(Action<(string, T[])> onComplete, Func<T, char> toChar)
        : RegexMatcher<T>(onComplete, toChar, @"\A[^\x00\x1B]+\z");
}