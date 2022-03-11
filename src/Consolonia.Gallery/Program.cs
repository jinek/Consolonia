using System;
using Consolonia.Core;

namespace Consolonia.Gallery
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 1) throw new NotSupportedException();

            ApplicationStartup.StartConsolonia<App>();
        }
    }
}