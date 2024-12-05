# Consolonia UI
TUI (Text User Interface) (GUI Framework) implementation for [Avalonia UI](https://github.com/AvaloniaUI)

Supports XAML, data bindings, animation, styling and the rest from Avalonia.

# Showcase (click picture to see video)
[![datagridpic](https://user-images.githubusercontent.com/10516222/141980173-4eb4057a-6996-45bf-83f6-931316c98d88.png)](https://youtu.be/ttgZmbruk3Y)

This package is the core Consolonia library.

# Usage
Define an application with a theme (See Consolonia.Themes.TurboVision for themes)

## Define a Window
HelloWorldWindow.axaml
```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="HelloWorldWindow">
    <TextBlock Text="Hello World">
</Window>
```

## Define an application 
You need to define an application that defines a theme and sets the main window.

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


## Setup program.cs

Program.cs
```csharp
[STAThread]
private static void Main(string[] args)
{
    BuildAvaloniaApp()
        .StartWithConsoleLifetime(args);
}

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<HelloWorldApp>()
        .UseConsolonia()
        .UseAutoDetectedConsole()
        .LogToException();
```

> **NOTE:** 
> * You need package **Consolonia**.**Themes**.**TurboVision** for themes
> * You need package **Consolonia**.**PlatformSupport** to add an IConsole implementation via the .UseAutoDetectedConsole() method
 