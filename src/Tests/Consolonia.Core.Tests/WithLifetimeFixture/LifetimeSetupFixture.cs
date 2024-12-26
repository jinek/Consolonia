using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Dummy;
using NUnit.Framework;
using static System.GC;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [SetUpFixture]
    public class LifetimeSetupFixture : IDisposable
    {
        private class ContextApp2 : Application;
        
        [OneTimeSetUp]
        public void Setup()
        {//todo: setup copypasted from another test. Can be extracted to a base class?
            _scope = AvaloniaLocator.EnterScope();
            _lifetime = ApplicationStartup.BuildLifetime<ContextApp2>(new DummyConsole(), new RgbConsoleColorMode(), []);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispose();
        }
        
        private IDisposable _scope;
        private ClassicDesktopStyleApplicationLifetime _lifetime;
        private bool _disposedValue;
        
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
    }
}