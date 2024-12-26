using System;
using Consolonia.Core.Infrastructure;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        public void TestExectpionRequest()
        {
            var request = new NotSupportedRequest(5, Array.Empty<object>());
            try
            {
                throw new ConsoloniaNotSupportedException(request);
            }
            catch (ConsoloniaNotSupportedException ex)
            {
                Assert.AreEqual(request, ex.Request);
            }
        }
    }
}