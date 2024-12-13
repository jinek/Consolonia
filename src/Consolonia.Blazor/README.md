# Consolonia.Blazor
This package provides the ability to use Blazor .razor files to create Consolonia applications.
* Use [⚡ Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) syntax for [Avalonia](https://avaloniaui.net/) apps
* 😎 Simpler syntax than XAML
* 🪄 IntelliSense support
* Get free [🔥 Hot Reload](https://devblogs.microsoft.com/dotnet/introducing-net-hot-reload/) support on-top
* See https://github.com/Epictek/Avalonia-Blazor-Bindings

## Background
Consolonia is a TUI (Text User Interface) (GUI Framework) implementation for [Avalonia UI](https://github.com/AvaloniaUI)

## Showcase (click picture to see video)
[![datagridpic](https://user-images.githubusercontent.com/10516222/141980173-4eb4057a-6996-45bf-83f6-931316c98d88.png)](https://youtu.be/ttgZmbruk3Y)


## Example Razor file
Things to note:
* Method binding to code behind
* Conditional layout of elements
* Full intellisense for renaming going to code behind autocomplete etc.
* injection of **INavigation** so you can push and pop views off of the nav stack
* injection of **IClassicDesktopStyleApplicationLifetime** so you can access Args and shutdown the app.
* HOT RELOAD. simple save the file and your view gets rebuilt and rerendered!
 
![blazor](https://github.com/user-attachments/assets/ae1ba484-b3a9-46c6-8c1b-99026e7f924c)

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
