![logo](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Logo.png)

# Consolonia.NUnit
This package provides testing support for testing consolonia controls using NUnit.

## Usage
To create a unit test against your consolonia application your test class should derive from ConsoloniaAppTestBase&lt;App$gt;.

You can define the size of your console by passing size parameter to base class.

Then you can use UITest to interact with your application, and use UITest.AssertHasText() to verify the screen of text matches your expectation.

### UITest.AssertHasText()
Takes one or more Regex patterns and verifies that the screen contains the text that matches the pattern.

### UITest.AssertHasNoText()
Takes one or more Regex patterns and verifies that the screen does not contain the text that matches the pattern.

## Example
```csharp
    public class Tests : ConsoloniaAppTestBase<App>
    {
        public Tests : base(new PixelBufferSize(80, 40))
        {
        }


        [Test]
        public async Task DisplaysBasicText()
        {
            await UITest.KeyInput(Key.Tab);
            await UITest.AssertHasText("This is TextBlock");
        }
    }
```
