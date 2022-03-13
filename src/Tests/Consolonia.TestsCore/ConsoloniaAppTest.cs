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
    public abstract class ConsoloniaAppTest<TApp> where TApp : Application, new()
    {
        private readonly PixelBufferSize _size;
        private ClassicDesktopStyleApplicationLifetime _lifetime;
        private IDisposable _scope;
        protected UnitTestConsole UITest;

        protected ConsoloniaAppTest(PixelBufferSize size)
        {
            _size = size;
        }

        protected string[] Args { get; init; }

        [SetUp]
        public async Task Setup()
        {
            UITest = new UnitTestConsole(_size);
            bool setup = false;
            Task.Run(() =>
            {
                _scope = AvaloniaLocator.EnterScope();
                _lifetime = ApplicationStartup.BuildLifetime<TApp>(UITest, Args);
                UITest.SetupLifetime(_lifetime);
                Dispatcher.UIThread.UpdateServicesExtension();
                setup = true;
                _lifetime.Start(Args);

                // Resetting static of AppBuilderBase
                typeof(AppBuilderBase<AppBuilder>).GetField("s_setupWasAlreadyCalled",
                        BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(null, false);
            }).ContinueWith(_ => { ThreadPool.QueueUserWorkItem(_ => throw new NotImplementedException()); },
                TaskContinuationOptions.OnlyOnFaulted);

            while (!setup) /*todo: replace by semaphore/WaitHandler*/ ;

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
            }, cancellationToken);

            // Waiting all jobs to finish
            await UITest.WaitDispatched();
        }

        [TearDown]
        public async Task TearDown()
        {
            await Dispatcher.UIThread.InvokeAsync(() => { _lifetime.Shutdown(); });

            _lifetime.Dispose();
            _lifetime = null;
            _scope.Dispose();
            _scope = null;
            Dispatcher.UIThread.UpdateServicesExtension();
            UITest.Dispose();
            UITest = null;
        }
    }
}