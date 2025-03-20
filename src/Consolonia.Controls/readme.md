# Consolonia.Controls
Consolonia is a TUI (Text User Interface) (GUI Framework) implementation for [Avalonia UI](https://github.com/AvaloniaUI)

Supports XAML, data bindings, animation, styling and the rest from Avalonia.

# Showcase (click picture to see video)
[![datagridpic](https://user-images.githubusercontent.com/10516222/141980173-4eb4057a-6996-45bf-83f6-931316c98d88.png)](https://youtu.be/ttgZmbruk3Y)

This package contains the following Consolonia Themes:
* **MaterialTheme** - Material Design theme
* **FluentTheme** - Fluent Design theme
* **TurboVisionTheme** - TurboVision theme
* **TurboVisionDarkTheme** - TurboVision Dark theme
* **TurboVisionBlackTheme** - TurboVision Black theme

# Usage
Define an application with a theme 

## Define an application 
Themes are styles which are applied to the application.

HelloWorldApp.cs
```csharp
// use HelloWorldWindow as the MainWindow for the application
public class HelloWorldApp : ConsoloniaApplication<HelloWorldWindow>
{
    public override void Initialize()
    {
        // set the theme
        Styles.Add(new MaterialTheme());
    }
}
```

