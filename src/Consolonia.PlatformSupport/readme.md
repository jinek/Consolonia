![logo](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Logo.png)

# Consolonia.PlatformSupport
This package provides platform support for Windows, Linux and MacOS environments when building Console applications using [Consolonia](https://github.com/jinek/consolonia)
and [Avalonia](https://avaloniaui.net/).

![gallery](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Gallery.gif)


## Usage
To use this package, you need to call the `UseConsolonia` and `UseAutoDetectedConsole` extension methods on the `AppBuilder` instance in your `Program.cs` file.

```csharp
AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        .UseConsolonia()
        .UseAutoDetectedConsole();
}
```