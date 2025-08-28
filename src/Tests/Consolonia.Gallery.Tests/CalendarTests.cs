using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class CalendarTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("24 25 26 27 28 29 30");
            await UITest.AssertHasText(@"April", @"2022");
            await UITest.KeyInput(Key.Tab, Key.Back);
            await UITest.AssertHasText(@"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov",
                "Dec");
            await UITest.KeyInput(Key.Back);
            await UITest.AssertHasText(Enumerable.Range(2019, 12).Select(year => year.ToString()).ToArray());
            await UITest.KeyInput(Key.Down, Key.Enter, Key.Down, Key.Enter);
            await UITest.AssertHasText("August", "2026", "26 27 28 29 30 31 1");
        }
    }
}