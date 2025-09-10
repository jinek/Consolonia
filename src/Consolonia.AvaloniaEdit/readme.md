![](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Logo.png)

# Consolonia.AvaloniaEdit
This package contains theme resources for using **AvaloniaEdit** in [Consolonia](https://github.com/jinek/consolonia)applications.

![](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/AvaloniaEdit.gif)

# Usage
To enable AvaloniaEdit in your application:
1. Add **Avalonia.AavaloniaEdit** nuget package
1. Add **Consolonia.AvaloniaEdit** nuget package
1. Add ```avares://Consolonia.AvaloniaEdit/Theme.axaml``` to your application styles.

```xaml
    <Application.Styles>
        <console:ModernTheme />
        <StyleInclude Source="avares://Consolonia.AvaloniaEdit/Theme.axaml" />
    </Application.Styles>

```

