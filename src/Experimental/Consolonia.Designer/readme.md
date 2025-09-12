# Consolonia.Designer
This package **EXPERIMENTAL** Consolonia support for design time previews.

## Usage
```csharp
[STAThread]
private static void Main(string[] args)
{
    BuildAvaloniaApp()
        .StartWithConsoleLifetime(args);
}

public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UseConsoloniaDesigner()
        .UseAutoDetectedConsole()
        .LogToException();
```

