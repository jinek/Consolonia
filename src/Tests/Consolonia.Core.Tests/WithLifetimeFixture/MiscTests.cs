using System;
using Consolonia.Core.Infrastructure;
using NUnit.Framework;

namespace Consolonia.Core.Tests.WithLifetimeFixture
{
    [TestFixture]
    public class MiscTests
    {
        [Test]
        public void TestExceptionRequest()
        {
            const int supportedRequestCode = -1;
            const NotSupportedRequestCode notSupportedRequestCode = (NotSupportedRequestCode)supportedRequestCode;
            Assert.IsFalse(Enum.IsDefined(notSupportedRequestCode),
                $"Code {supportedRequestCode} is reserved for internal use: tests");

            var request = new NotSupportedRequest(notSupportedRequestCode, Array.Empty<object>());
            try
            {
                throw new ConsoloniaNotSupportedException(request, typeof(object));
            }
            catch (ConsoloniaNotSupportedException ex)
            {
                Assert.AreEqual(request.ErrorCode, ex.Request.ErrorCode);
                Assert.IsFalse(request.Handled);
            }
        }
    }
}