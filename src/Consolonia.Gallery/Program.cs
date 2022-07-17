using System;
using Consolonia.Core;

namespace Consolonia.Gallery
{
    internal static class Program
    {
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local Exactly why we are keeping it here
        private static void Main(string[] args)
        {
            if (args.Length > 1) throw new NotSupportedException();

            ApplicationStartup.StartConsolonia<App>(args);
        }
    }
}