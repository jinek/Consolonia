using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Drawing.PixelBufferImplementation.EgaConsoleColor;

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
                mainWindowPlatformImpl.Console.ClearOutput();

                Dispatcher.UIThread.Post(() => { MainWindow.InvalidateVisual(); });
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return Dispatcher.UIThread.InvokeAsync(() => { }).GetTask();
        }

        public bool IsRgbColorMode()
        {
            IConsoleColorMode consoleColorMode = AvaloniaLocator.Current.GetService<IConsoleColorMode>()
                                                 ?? throw new ConsoloniaException("Console color mode has not been initialized");

            return consoleColorMode is RgbConsoleColorMode;
        }
    }
}