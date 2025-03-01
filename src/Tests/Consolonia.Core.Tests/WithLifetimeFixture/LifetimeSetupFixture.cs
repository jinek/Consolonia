using System;
using Avalonia;
using Consolonia.Controls;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Dummy;
using Consolonia.Core.Infrastructure;
using Consolonia.NUnit;
using NUnit.Framework;
using static System.GC;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [SetUpFixture]
    public class LifetimeSetupFixture : IDisposable
    {
        private bool _disposedValue;
        private ConsoloniaLifetime _lifetime;

        private IDisposable _scope;

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DrawingContextImplTests()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            SuppressFinalize(this);
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var console = new UnitTestConsole(new PixelBufferSize(100, 100));
            AvaloniaLocator.Current = new AvaloniaLocator()
                .Bind<IConsoleOutput>().ToConstant(console)
                .Bind<IConsoleCapabilities>().ToConstant(console);

            _scope = AvaloniaLocator.EnterScope();
            _lifetime = ApplicationStartup.BuildLifetime<ContextApp2>(new DummyConsole(), new RgbConsoleColorMode(),
                []);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _scope?.Dispose();
                    _lifetime?.Dispose();
                    _scope = null;
                    _lifetime = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        private class ContextApp2 : Application;
    }
}