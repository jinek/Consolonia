using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.NUnit
{
    [NonParallelizable /*todo: switch to semaphore like https://stackoverflow.com/a/6427425/2362847 to allow other tests to execute in parallel*/]
#pragma warning disable CA1001 // we are relying on TearDown by NUnit
    public abstract class ConsoloniaAppTestBase<TApp>
        where TApp : Application, new()
#pragma warning restore CA1001
    {
        private readonly PixelBufferSize _size;
        private IDisposable _scope;

        protected ConsoloniaAppTestBase(PixelBufferSize size)
        {
            _size = size;
        }


#pragma warning disable CA1819 // todo: provide a solution
        protected string[] Args { get; init; }
#pragma warning restore CA1819

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            if (UITest != null)
                return;

            AppDomain.CurrentDomain.ProcessExit += GlobalTearDown;

            UITest = new UnitTestConsole(_size);
            var setupTaskSource = new TaskCompletionSource();

            ThreadPool.QueueUserWorkItem(_ =>
            {
                _disposeTaskCompletionSource = new TaskCompletionSource();
                _scope = AvaloniaLocator.EnterScope();
                _lifetime = ApplicationStartup.BuildLifetime<TApp>(UITest, new RgbConsoleColorMode(), Args);
                UITest.SetupLifetime(_lifetime);
                setupTaskSource.SetResult();
                _lifetime.Start(Args);

                // Resetting static of AppBuilderBase
                typeof(AppBuilder).GetField("s_setupWasAlreadyCalled",
                        BindingFlags.Static | BindingFlags.NonPublic)!
                    .SetValue(null, false);
                _disposeTaskCompletionSource.SetResult();
            });

            await setupTaskSource.Task.ConfigureAwait(true);

            // Waiting Main Window To appear
            CancellationToken cancellationToken = new CancellationTokenSource(60000 /*todo: magic number*/).Token;
            await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    bool windowFound = await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        Window mainWindow = _lifetime?.MainWindow;
                        return mainWindow != null;
                    });
                    if (windowFound)
                        return;
                }
            }, cancellationToken).ConfigureAwait(true);

            // Waiting all jobs to finish
            await UITest.WaitRendered().ConfigureAwait(true);
        }

        private async void GlobalTearDown(object sender, EventArgs eventArgs)
        {
            AppDomain.CurrentDomain.ProcessExit -= GlobalTearDown;

            ClassicDesktopStyleApplicationLifetime lifetime = _lifetime;
            await Dispatcher.UIThread.InvokeAsync(() => { lifetime.Shutdown(); }).GetTask().ConfigureAwait(true);

            _lifetime.Dispose();
            _lifetime = null;
            _scope.Dispose();
            _scope = null;
            UITest.Dispose();
            UITest = null;
            await _disposeTaskCompletionSource.Task.ConfigureAwait(true);
        }

        // ReSharper disable StaticMemberInGenericType
        private static TaskCompletionSource _disposeTaskCompletionSource; // todo: tests now rely on static
        private static ClassicDesktopStyleApplicationLifetime _lifetime;

        protected static UnitTestConsole UITest { get; private set; }
        // ReSharper restore StaticMemberInGenericType
    }
}