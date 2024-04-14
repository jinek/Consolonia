using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

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

            var mainWindowPlatformImpl = (ConsoleWindow)MainWindow.PlatformImpl;
            IConsole console = mainWindowPlatformImpl.Console;

            Task pauseTask = taskToWaitFor.Task;

            console.PauseIO(pauseTask);

            pauseTask.ContinueWith(_ =>
            {
                mainWindowPlatformImpl.Console.ClearOutput();

                Dispatcher.UIThread.Post(() => { MainWindow.InvalidateVisual(); });
            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);

            return Dispatcher.UIThread.InvokeAsync(() => { }).GetTask();
        }
    }
}