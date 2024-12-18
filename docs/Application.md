# Consolonia Application
The ConosoloniaApplication is just like the normal Avalonia Application, but with some extra features.

The main task for an application is to
* Define the initial window 
* Defining the application resources (like styles, themes, etc.)

## Creating the initial window 
The MainWindow is the main window of the application which derives from `Window` class. 
You can do it like a traditional avalonia app:
```csharp
public class App : ConsoloniaApplication
{
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MyMainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
```
or you can use the ConsoloniaApplication<T> class to make it easier:

```csharp
public class MyApplication : ConsoloniaApplication<MyMainWindow>
{
    public MyApplication()
    {
        // Initialize the application
    }
}
```


## Define application resources 
The application has a global ResourceDictionary and Styles collection which allows you to set global resources for your application. 
It is very common to define the theme in the Application. 

```csharp
class MyApp : ConsoloniaApplication<MyMainWindow>
{
    public MyApp()
    {
        Styles.Add(new MaterialTheme());
    }
}
```

# References 
* [Overview](/docs)
* [Quick Start](/docs/QuickStart.md)
* [Application](/docs/Application.md)
* [Dialogs](/docs/Dialogs.md)
* [Blazor](/docs/Blazor.md)

