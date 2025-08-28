using System;

namespace Consolonia.Core.Helpers.InputProcessing
{
    /// <summary>
    ///     A matcher that matches any text input.
    /// </summary>
    public class TextInputMatcher<T>(Action<(string, T[])> onComplete, Func<T, char> toChar, uint? min = null)
        : RegexMatcher<T>(onComplete, toChar, @"\A[^\x00\x1B]+\z")
    {
        public override bool TryFlush()
        {
            if (min != null && GetAccumulatedLength() < (uint)min)
                return false;

            return base.TryFlush();
        }
    }
}