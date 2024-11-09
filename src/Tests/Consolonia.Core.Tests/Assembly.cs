using System;
using Avalonia;
using Consolonia.Core.Drawing.PixelBufferImplementation;
using Consolonia.Core.Infrastructure;
using Consolonia.NUnit;
using NUnit.Framework;

[assembly: CLSCompliant(false)] //todo: should we make it compliant?

namespace Consolonia.Core.Tests
{
    [SetUpFixture]
    public class AllTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            AvaloniaLocator.Current = new AvaloniaLocator()
                .Bind<IConsole>().ToConstant(new UnitTestConsole(new PixelBufferSize(100, 100)));
        }
    }
}