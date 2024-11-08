using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core.Text;

namespace Consolonia.Core.Infrastructure
{
    public class ConsoloniaLifetime : ClassicDesktopStyleApplicationLifetime
    {
        public ConsoloniaLifetime()
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.Write(ConsoleUtils.EnableAlternateBuffer);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            Exit += ConsoloniaLifetime_Exit;
        }

        private void ConsoloniaLifetime_Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.Write(ConsoleUtils.DisableAlternateBuffer);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

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


    }
}