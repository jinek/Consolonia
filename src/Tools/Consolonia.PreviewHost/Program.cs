using System.Globalization;
using System.Text;
using Avalonia;
using Consolonia.Core;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.PlatformSupport;

namespace Consolonia.PreviewHost
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            AppBuilder? builder = AppBuilder.Configure<App>()
                .UseConsolonia()
                .LogToException();

            if (args.Contains("--buffer"))
            {
                string[] parts = args.SkipWhile(a => a != "--buffer").Skip(1).Take(2).ToArray();
                ushort width = ushort.Parse(parts[0], CultureInfo.InvariantCulture);
                ushort height = ushort.Parse(parts[1], CultureInfo.InvariantCulture);
                builder = builder.UseConsole(new DummyConsole(width, height));
            }
            else
            {
                builder = builder.UseAutoDetectedConsole();
            }

            builder
                .StartWithConsoleLifetime(args);
        }
    }
}