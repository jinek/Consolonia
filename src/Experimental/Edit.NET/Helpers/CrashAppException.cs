using System;

namespace EditNET.Helpers
{
    /// <summary>
    ///     Exception which crashes the application
    /// </summary>
    internal class CrashAppException(Exception innerException)
        : Exception("We apologize for the inconvenience, but an unexpected error occurred.", innerException);
}