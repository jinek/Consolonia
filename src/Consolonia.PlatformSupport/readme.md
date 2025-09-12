![logo](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Logo.png)

# Consolonia.PlatformSupport
This package provides platform support for Windows, Linux, and macOS when building console applications using 
[Consolonia](https://github.com/jinek/consolonia) and [Avalonia](https://avaloniaui.net/).

![gallery](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Gallery.gif)


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