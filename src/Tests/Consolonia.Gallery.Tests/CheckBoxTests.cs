using System.Threading.Tasks;
using Avalonia.Input;
using Consolonia.Gallery.Tests.Base;
using Consolonia.NUnit;
using NUnit.Framework;

namespace Consolonia.Gallery.Tests
{
    [TestFixture]
    internal class CheckBoxTests : GalleryTestsBaseBase
    {
        [Test]
        public async Task PerformSingleTest()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText(
                "☐.+Unchecked",
                "🗹.+Checked",
                @"■.+Indeterminate",
                "Disabled",
                "☐.+Three State: Unchecked",
                "🗹.+Three State: Checked",
                @"■.+Three State: Indeterminate",
                @"■.+Three State: Disabled"
            );

            for (int i = 0; i < 6; i++) await UITest.KeyInput(Key.Space, Key.Tab);

            await UITest.AssertHasText(
                "🗹.+Unchecked",
                "☐.+Checked",
                "☐.+Indeterminate",
                "Disabled",
                "🗹.+Three State: Unchecked",
                @"■.+Three State: Checked",
                "☐.+Three State: Indeterminate",
                @"■.+Three State: Disabled"
            );
        }
    }
}