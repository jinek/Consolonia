![logo](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Logo.png)

# Consolonia.Themes.TurboVision
This package provides themes when creating console apps using [Consolonia](https://github.com/jinek/consolonia).

![themes](https://raw.githubusercontent.com/jinek/consolonia/main/assets/images/Themes.gif)

This package contains the following Consolonia Themes:
* **ModernTheme** - Modern  theme
* **TurboVisionTheme** - TurboVision theme
* **TurboVisionCompatibleTheme** - TurboVision theme identical to TurboVisionDark
* **TurboVisionGrayTheme** - TurboVision theme identical to TurboVisionDark
* **TurboVisionElegantTheme** - TurboVision theme identical to TurboVisionDark


# Usage
You add themes via application styles and RequestedThemeVariant=[Light|Dark|Default]

App.axaml
```xaml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:console="https://github.com/jinek/consolonia"
             RequestedThemeVariant="Default">
    <Application.Styles>
        <console:ModernTheme />
    </Application.Styles>
</Application>
```

