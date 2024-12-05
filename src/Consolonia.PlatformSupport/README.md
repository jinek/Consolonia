# Consolonia.PlatformSupport
This package provides Consolonia IConsole platform support for adapting to consoles on Windows, Linux and MacOS environments.

## Background
Consolonia is a TUI (Text User Interface) (GUI Framework) implementation for [Avalonia UI](https://github.com/AvaloniaUI)

Supports XAML, data bindings, animation, styling and the rest from Avalonia.

## Showcase (click picture to see video)
[![datagridpic](https://user-images.githubusercontent.com/10516222/141980173-4eb4057a-6996-45bf-83f6-931316c98d88.png)](https://youtu.be/ttgZmbruk3Y)

## Usage
```csharp
AppBuilder BuildAvaloniaApp()
{
    return AppBuilder.Configure<App>()
        .UseConsolonia()
        .UseAutoDetectedConsole();
}
```