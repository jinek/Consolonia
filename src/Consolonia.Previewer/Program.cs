using System.Text;
using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Infrastructure;
using Consolonia.Dummy;
using Consolonia.PlatformSupport;

namespace Consolonia.Previewer
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            var builder = AppBuilder.Configure<App>()
                .UseConsolonia()
                .LogToException();
            
            if (args.Contains("--buffer"))
            {
                var parts = args.SkipWhile(a => a != "--buffer").Skip(1).Take(2).ToArray();
                var width = ushort.Parse(parts[0]);
                var height = ushort.Parse(parts[1]);
                builder = builder.UseConsole(new DummyConsole(width, height));
            }
            else
                builder = builder.UseAutoDetectedConsole();
            
            builder
                .StartWithConsoleLifetime(args);
        }
    }
}
