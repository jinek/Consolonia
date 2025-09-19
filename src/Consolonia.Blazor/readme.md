![logo](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Logo.png)

# Consolonia.Blazor
This package provides the ability to use Blazor .razor files to create console apps using [Consolonia](https://github.com/jinek/consolonia).
* Use [⚡ Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) syntax for [Avalonia](https://avaloniaui.net/) apps
* 😎 Simpler syntax than XAML
* 🪄 IntelliSense support
* Get free [🔥 Hot Reload](https://devblogs.microsoft.com/dotnet/introducing-net-hot-reload/) support on-top
* See https://github.com/Epictek/Avalonia-Blazor-Bindings

![blazor](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Blazor.gif)

## Example Razor file
Things to note:
* Method binding to code behind
* Conditional layout of elements
* Full intellisense for renaming going to code behind autocomplete etc.
* injection of **INavigation** so you can push and pop views off of the nav stack
* injection of **IClassicDesktopStyleApplicationLifetime** so you can access Args and shutdown the app.
* HOT RELOAD. simple save the file and your view gets rebuilt and rerendered!
 

```razor
@page "/"
@inject INavigation navigation
@inject IClassicDesktopStyleApplicationLifetime lifetime

@namespace Example.Blazor.Components

<Window Title="Blazor Bindings for Consolonia" Topmost="true">
     <StackPanel Orientation="Orientation.Vertical" VerticalAlignment="VerticalAlignment.Center">
        @if (showCounter)
        {
            <TextBlock HorizontalAlignment="HorizontalAlignment.Center">Counter: @CounterText</TextBlock>
        }
        <StackPanel Orientation="Orientation.Vertical" HorizontalAlignment="HorizontalAlignment.Center" Margin="@Thickness.Parse("4")">
            <Button OnClick="@OnIncrement">Increment counter support</Button>
            <Button OnClick="@OnToggleCounter">@ToggleText</Button>
            <Button OnClick="@OnMessageBox">Message Box</Button>
            <Button OnClick="@OnGotoSubPage">Go to SubPage</Button>
            <Button OnClick="@OnExit">Exit</Button>
        </StackPanel>
    </StackPanel> 
</Window>

@code {
    int counter = 0;
    bool showCounter = true;

    string CounterText => counter switch
    {
        0 => "Not clicked",
        1 => $"Clicked 1 time",
        _ => $"Clicked {counter} times"
    };

    string ToggleText => showCounter ? "Hide Counter" : "Show Counter";

    void OnIncrement()
        => counter++;

    void OnToggleCounter()
        => showCounter = !showCounter;

    void OnGotoSubPage()
        => navigation.PushAsync<SubPage>();

    async void OnMessageBox(RoutedEventArgs args)
    {
        await new MessageBox()
            .ShowDialogAsync((AC.Control)args.Source!, "Hello, Blazor!", "Blazor Bindings for Consolonia");
    }

    void OnExit()
       => lifetime.Shutdown();
}
```
