# Quick Start
This is a quick start tutorial for creating a Consolonia application.

# 1. Create a new project
Create a new console based application by running the following command:
```bash
dotnet new console -n MyConsoloniaApp
```

# 2. Add Consolonia to the project
Add Consolonia nuget to the project
```bash
dotnet add package Consolonia
```

# 3. Edit program.cs to use Consolonia
Add the following code to the `Program.cs` file
```csharp   
using System;
using Consolonia;

namespace MyConsoloniaApp
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildAvaloniaApp()
                .StartWithConsoleLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<MyApp>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .LogToException();
        }
    }
}
```

# 4. Create a MainWindow.axaml file
Create a new file called `MainWindow.axaml` in the project directory with the following content:
```xaml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Hello World Application"
        x:Class="HelloWorld.MainWindow">
    Hello world!
</Window>
```

# 5. Create MainWindow.axaml.cs file
Create a new file called `MainWindow.axaml.cs` in the project directory with the following content:
```csharp
using Avalonia.Controls;

namespace HelloWorld
{
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            InitializeComponent();
        }
    }
}
```

# 6. Create a new App.cs file
Create a new file called `MyApp.cs` in the project directory with the following content:
```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace HelloWorld
{
    public class MyApp : ConsoloniaApplication<MainWindow>
    {
    }
}
```

# 7. Run the application
Run the application by executing the following command:
```bash
dotnet run
```

# References 
* [Overview](/docs)
* [Quick Start](/docs/QuickStart.md)
* [Application](/docs/Application.md)
* [Dialogs](/docs/Dialogs.md)
* [Blazor](/docs/Blazor.md)

