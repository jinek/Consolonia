namespace Consolonia.Core.Helpers.InputProcessor
{
    /// <summary>
    ///     Interface for a matcher that accumulates input and tries to match it to a pattern.
    /// </summary>
    /// <typeparam name="T">The type of input to match.</typeparam>
    public interface IMatcher<in T>
    {
        AccumulationResult Accumulate(T input);
        bool TryFlush();
        void Reset();
    }
}