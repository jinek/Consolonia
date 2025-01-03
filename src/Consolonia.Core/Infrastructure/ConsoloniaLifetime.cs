using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaLifetime : ClassicDesktopStyleApplicationLifetime
    {
        /// <summary>
        ///     returned task indicates that console is successfully paused
        /// </summary>
        public Task DisconnectFromConsoleAsync(CancellationToken cancellationToken)
        {
            var taskToWaitFor = new TaskCompletionSource();
            cancellationToken.Register(() => taskToWaitFor.SetResult());

            var mainWindowPlatformImpl = (ConsoleWindow)MainWindow!.PlatformImpl;
            IConsole console = mainWindowPlatformImpl!.Console;

            Task pauseTask = taskToWaitFor.Task;

            console.PauseIO(pauseTask);

            pauseTask.ContinueWith(_ =>
            {
                mainWindowPlatformImpl.Console.ClearScreen();

                Dispatcher.UIThread.Post(() => { MainWindow.InvalidateVisual(); });
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return Dispatcher.UIThread.InvokeAsync(() => { }).GetTask();
        }

#pragma warning disable CA1822
        // ReSharper disable once MemberCanBeMadeStatic.Global It can be static only because we rely on static locator currently. I bet it will change in the future
        public bool IsRgbColorMode()
#pragma warning restore CA1822
        {
            IConsoleColorMode consoleColorMode = AvaloniaLocator.Current.GetService<IConsoleColorMode>()
                                                 ?? throw new ConsoloniaException(
                                                     "Console color mode has not been initialized");

            return consoleColorMode is RgbConsoleColorMode;
        }
    }
}