using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.GalleryTests.Base;
using Consolonia.TestsCore;
using NUnit.Framework;

namespace Consolonia.GalleryTests
{
    [TestFixture]
    internal class CheckBoxTests : GalleryTestsBaseBase
    {
        protected override async Task PerformSingleTest()
        {
            await UITest.AssertHasText(
                @"[ ].+Unchecked",
                @"[V].+Checked",
                @"[■].+Indeterminate",
                @"Disabled",
                @"[ ].+Three State: Unchecked",
                @"[V].+Three State: Checked",
                @"[■].+Three State: Indeterminate",
                @"[■].+Three State: Disabled"
            );

            for (int i = 0; i < 6; i++) await UITest.KeyInput(Key.Space, Key.Tab);

            await UITest.AssertHasText(
                @"[V].+Unchecked",
                @"[ ].+Checked",
                @"[ ].+Indeterminate",
                @"Disabled",
                @"[V].+Three State: Unchecked",
                @"[■].+Three State: Checked",
                @"[ ].+Three State: Indeterminate",
                @"[■].+Three State: Disabled"
            );
        }
    }
}