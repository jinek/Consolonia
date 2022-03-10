using System;

namespace Consolonia.Gallery
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1) throw new NotSupportedException();

            Core.ApplicationStartup.StartConsolonia<App>();
        }
    }
}