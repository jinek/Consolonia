using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Consolonia.Core;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using NUnit.Framework;

namespace Consolonia.TestsCore
{
    [NonParallelizable/*todo: switch to semaphore like https://stackoverflow.com/a/6427425/2362847 to allow other tests to execute in parallel*/]
#pragma warning disable CA1001 // we are relying on TearDown by NUnit
    public abstract class ConsoloniaAppTestBase<TApp> where TApp : Application, new()
#pragma warning restore CA1001
    {
        private readonly PixelBufferSize _size;
        private ClassicDesktopStyleApplicationLifetime _lifetime;
        private IDisposable _scope;
        protected UnitTestConsole UITest { get; private set; }
        private TaskCompletionSource _disposeTaskCompletionSource;

        protected ConsoloniaAppTestBase(PixelBufferSize size)
        {
            _size = size;
        }

#pragma warning disable CA1819 // todo: provide a solution
        protected string[] Args { get; init; }
#pragma warning restore CA1819

        [SetUp]
        public async Task Setup()
        {
            UITest = new UnitTestConsole(_size);
            var setupTaskSource = new TaskCompletionSource();
            
            ThreadPool.QueueUserWorkItem(_ =>
            {
                _disposeTaskCompletionSource = new TaskCompletionSource();
                _scope = AvaloniaLocator.EnterScope();
                _lifetime = ApplicationStartup.BuildLifetime<TApp>(UITest, Args);
                UITest.SetupLifetime(_lifetime);
                Dispatcher.UIThread.UpdateServicesExtension();
                setupTaskSource.SetResult();
                _lifetime.Start(Args);

                // Resetting static of AppBuilderBase
                typeof(AppBuilderBase<AppBuilder>).GetField("s_setupWasAlreadyCalled",
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
                    }).ConfigureAwait(false);
                    if (windowFound)
                        return;
                }
            }, cancellationToken).ConfigureAwait(true);

            // Waiting all jobs to finish
            await UITest.WaitDispatched().ConfigureAwait(true);
        }

        [TearDown]
        public async Task TearDown()
        {
            await Dispatcher.UIThread.InvokeAsync(() => { _lifetime.Shutdown(); }).ConfigureAwait(true);

            _lifetime.Dispose();
            _lifetime = null;
            _scope.Dispose();
            _scope = null;
            Dispatcher.UIThread.UpdateServicesExtension();
            UITest.Dispose();
            UITest = null;
            await _disposeTaskCompletionSource.Task.ConfigureAwait(true);
        }
    }
}