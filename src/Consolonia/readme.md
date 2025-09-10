![](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Logo.png)

# 🚀 Modern UI Power, Now in Your Console
This package is meta package for building Console applications using [Consolonia](https://github.com/jinek/consolonia)
and [Avalonia](https://avaloniaui.net/).

![](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Gallery.gif)

# Usage
Define an application with a theme (See Consolonia.Themes.TurboVision for themes)

## Define a Window
HelloWorldWindow.axaml
```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="HelloWorldWindow">
    <TextBlock Text="Hello World" />
</Window>

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
        Styles.Add(new FluentTheme());
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

 