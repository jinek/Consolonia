using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;

// ReSharper disable CheckNamespace
// ReSharper disable NotNullOrRequiredMemberIsNotInitialized
namespace Consolonia
{
    public class ConsoloniaLifetime : ISingleViewApplicationLifetime,
                                    IControlledApplicationLifetime,
                                    ISingleTopLevelApplicationLifetime,
                                    IDisposable
    {
        private int _exitCode;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private bool _isShuttingDown;
        private bool _disposedValue;

        public event EventHandler<ControlledApplicationLifetimeStartupEventArgs> Startup;

        public event EventHandler<ShutdownRequestedEventArgs> ShutdownRequested;

        public event EventHandler<ControlledApplicationLifetimeExitEventArgs> Exit;

        /// <summary>
        /// Gets the arguments passed to the AppBuilder Start method.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public string[] Args { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        public Control MainView
        {
            get => (Control)TopLevel.Content;
            set
            {
                if (value is TopLevel topLevel)
                    TopLevel = topLevel;
                else
                {
                    TopLevel = new Window()
                    {
                        Content = value,
                    };
                }
            }
        }

        public TopLevel TopLevel { get; set; }

        public void Shutdown(int exitCode = 0)
        {
            DoShutdown(new ShutdownRequestedEventArgs(), true, true, exitCode);
        }

        public bool TryShutdown(int exitCode = 0)
        {
            return DoShutdown(new ShutdownRequestedEventArgs(), true, false, exitCode);
        }

        internal void SetupCore(string[] args)
        {
            Startup?.Invoke(this, new ControlledApplicationLifetimeStartupEventArgs(args));

            var lifetimeEvents = AvaloniaLocator.Current.GetService<IPlatformLifetimeEventsImpl>();

            if (lifetimeEvents != null)
                lifetimeEvents.ShutdownRequested += OnShutdownRequested;

            TopLevel.Closed += (sender, args) => TryShutdown();
        }

        public int Start(string[] args)
        {
            return StartCore(args);
        }

        /// <summary>
        /// Since the lifetime must be set up/prepared with 'args' before executing Start(), an overload with no parameters seems more suitable for integrating with some lifetime manager providers, such as MS HostApplicationBuilder.
        /// </summary>
        /// <returns>exit code</returns>
        public int Start()
        {
            return StartCore(Args ?? Array.Empty<string>());
        }

        internal int StartCore(string[] args)
        {
            SetupCore(args);

            (TopLevel as Window)?.Show();

            Dispatcher.UIThread.MainLoop(_cts.Token);
            Environment.ExitCode = _exitCode;
            return _exitCode;
        }


        // ReSharper disable once UnusedMember.Local
#pragma warning disable IDE0060 // Remove unused parameter
        private bool DoShutdown(
            ShutdownRequestedEventArgs e,
            bool isProgrammatic,
            bool force = false,
            int exitCode = 0)
        {
            if (!force)
            {
                ShutdownRequested?.Invoke(this, e);

                if (e.Cancel)
                    return false;

                if (_isShuttingDown)
                    throw new InvalidOperationException("Application is already shutting down.");
            }

            _exitCode = exitCode;
            _isShuttingDown = true;
            var shutdownCancelled = false;

            ConsoleWindow consoleWindow = (ConsoleWindow)TopLevel.PlatformImpl;
            consoleWindow.Console.RestoreConsole();

            try
            {
                var args = new ControlledApplicationLifetimeExitEventArgs(exitCode);
                Exit?.Invoke(this, args);
                _exitCode = args.ApplicationExitCode;
            }
            finally
            {
                _isShuttingDown = false;

                if (!shutdownCancelled)
                {
                    _cts?.Cancel();
                    _cts = null;
                    Dispatcher.UIThread.InvokeShutdown();
                }
            }

            return true;
        }
#pragma warning restore IDE0060 // Remove unused parameter

        // ReSharper disable once UnusedMember.Local
        private void OnShutdownRequested(object sender, ShutdownRequestedEventArgs e) => DoShutdown(e, false);

        /// <summary>
        ///     returned task indicates that console is successfully paused
        /// </summary>
        public Task DisconnectFromConsoleAsync(CancellationToken cancellationToken)
        {
            var taskToWaitFor = new TaskCompletionSource();
            cancellationToken.Register(() => taskToWaitFor.SetResult());

            var mainWindowPlatformImpl = (ConsoleWindow)TopLevel.PlatformImpl;
            IConsole console = mainWindowPlatformImpl!.Console;

            Task pauseTask = taskToWaitFor.Task;

            console.PauseIO(pauseTask);

            pauseTask.ContinueWith(_ =>
            {
                mainWindowPlatformImpl.Console.ClearScreen();

                Dispatcher.UIThread.Post(() => { MainView.InvalidateVisual(); });
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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cts?.Dispose();
                    _cts = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ConsoloniaLifetime()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}