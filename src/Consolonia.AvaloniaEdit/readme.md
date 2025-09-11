![logo](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Logo.png)

# Consolonia.AvaloniaEdit
This package contains theme resources for using **AvaloniaEdit** in [Consolonia](https://github.com/jinek/consolonia) 
applications.

![avalonia editor](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/AvaloniaEdit.gif)

# Usage
To enable AvaloniaEdit in your application:
1. Add **Avalonia.AvaloniaEdit** NuGet package
1. Add **Consolonia.AvaloniaEdit** NuGet package
1. Add ```avares://Consolonia.AvaloniaEdit/Theme.axaml``` to your application styles.

```xaml
    <Application.Styles>
        <console:ModernTheme />
        <StyleInclude Source="avares://Consolonia.AvaloniaEdit/Theme.axaml" />
    </Application.Styles>

```

