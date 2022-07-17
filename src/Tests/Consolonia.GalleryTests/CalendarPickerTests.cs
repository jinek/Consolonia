using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class CalendarPickerTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            Console.WriteLine(Thread.CurrentThread.CurrentUICulture);
            Trace.WriteLine(Thread.CurrentThread.CurrentUICulture);
            
            await UITest.AssertHasText("2/16/2022");
            
            await UITest.KeyInput(Key.Left, Key.Delete);
            await UITest.StringInput("1");
            await UITest.KeyInput(Key.Enter);
            await UITest.AssertHasText("January", "2022", "16 17 18 19 20 21 22");
            await UITest.KeyInput(Key.Right);
            await UITest.AssertHasText("1/17/2022");
            await UITest.KeyInput(2, Key.OemMinus);
            await UITest.KeyInput(Key.Down, Key.Enter, Key.Down, Key.Down, Key.Enter, Key.Down, Key.Down, Key.Enter);
            await UITest.AssertHasText("9/15/2026");
        }
    }
}