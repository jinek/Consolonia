![logo](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Logo.png)

# Consolonia.Fonts
This package provides stock AsciiArt fonts for creating console apps using [Consolonia](https://github.com/jinek/consolonia).

# Ascii Art Fonts
Instead of TrueType/OpenType fonts, Consolonia supports AsciiArt fonts for rendering text in console applications. These fonts are made up of characters formed by multiple ASCII characters, allowing for a retro and stylized appearance.

There are 2 main formats for AsciiArt fonts supported by Consolonia:
* [Figlet Fonts](https://www.figlet.org/) - (.flf format)
* [Caca/Toilet fonts](http://caca.zoy.org/wiki/toilet) - (.tlf format)

# Installation and usage
Update Program.cs to call ```appBuilder.WithConsoleFonts()```
```csharp
      public static AppBuilder BuildAvaloniaApp()
        {
            return AppBuilder.Configure<App>()
                .UseConsolonia()
                .UseAutoDetectedConsole()
                .WithConsoleFonts(); // <---- Add this to add these fonts to your system.
        }
```

Then set your font family and fontsize whereever fonts can be used:

```xml
<TextBlock FontFamily="fonts:Consolonia#Standard" FontSize="8">...</TextBlock>
```


# Included Fonts
The following fonts are included in this package as embedded resources:


## fonts:Consolonia#WideTerm
Supported Font Sizes: 1
```
Ｃｏｎｓｏｌｏｎｉａ！
```

## fonts:Consolonia#Circle
Supported Font Sizes: 1
```
Ⓒⓞⓝⓢⓞⓛⓞⓝⓘⓐ!
```

## fonts:Consolonia#Braille
Supported Font Sizes: 2
```
 ⡎⠑ ⢀⡀ ⣀⡀ ⢀⣀ ⢀⡀ ⡇ ⢀⡀ ⣀⡀ ⠄ ⢀⣀ ⡇
 ⠣⠔ ⠣⠜ ⠇⠸ ⠭⠕ ⠣⠜ ⠣ ⠣⠜ ⠇⠸ ⠇ ⠣⠼ ⠅
```


## fonts:Consolonia#Digital
Supported Font Sizes: 3
```
+-+-+-+-+-+-+-+-+-+-+-+
|C|o|n|s|o|l|o|n|i|a|!|
+-+-+-+-+-+-+-+-+-+-+-+
```


## fonts:Consolonia#Emboss
Supported Font Sizes: 3

```
┏━┛┏━┃┏━ ┏━┛┏━┃┃  ┏━┃┏━ ┛┏━┃┃
┃  ┃ ┃┃ ┃━━┃┃ ┃┃  ┃ ┃┃ ┃┃┏━┃┛
━━┛━━┛┛ ┛━━┛━━┛━━┛━━┛┛ ┛┛┛ ┛┛
```

## fonts:Consolonia#Point
Supported Font Sizes: 3
```
/~` _  _  _ _ | _  _ . _ |
\_,(_)| |_\(_)|(_)| ||(_|.

```

## fonts:Console#Straight
Supported Font Sizes: 4
```
 __
/   _  _  _ _ | _  _ . _ |
\__(_)| )_)(_)|(_)| )|(_|.

```

## fonts:Consolonia#Mini
Supported Font Sizes: 4
```
 _
/   _  ._   _  _  |  _  ._  o  _. |
\_ (_) | | _> (_) | (_) | | | (_| o
```

## fonts:Consolonia#Standard
Supported Font Sizes: 5,6,8
```
  ____                      _             _       _
 / ___|___  _ __  ___  ___ | | ___  _ __ (_) __ _| |
| |   / _ \| '_ \/ __|/ _ \| |/ _ \| '_ \| |/ _` | |
| |__| (_) | | | \__ \ (_) | | (_) | | | | | (_| |_|
 \____\___/|_| |_|___/\___/|_|\___/|_| |_|_|\__,_(_)
```


## fonts:Consolonia#Rectangles
Supported Font Sizes: 6
```
                                       __
 _____                 _         _    |  |
|     |___ ___ ___ ___| |___ ___|_|___|  |
|   --| . |   |_ -| . | | . |   | | .'|__|
|_____|___|_|_|___|___|_|___|_|_|_|__,|__|

```
## fonts:Consolonia#Slant
Supported Font Sizes: 6
```
   ______                       __            _       __
  / ____/___  ____  _________  / /___  ____  (_)___ _/ /
 / /   / __ \/ __ \/ ___/ __ \/ / __ \/ __ \/ / __ `/ /
/ /___/ /_/ / / / (__  ) /_/ / / /_/ / / / / / /_/ /_/
\____/\____/_/ /_/____/\____/_/\____/_/ /_/_/\__,_(_)
```

## fonts:Console#Grafiiti
Supported Font Sizes: 6
```
_________                                 .__                   .__        ._.
\_   ___ \   ____    ____    ______ ____  |  |    ____    ____  |__|_____  | |
/    \  \/  /  _ \  /    \  /  ___//  _ \ |  |   /  _ \  /    \ |  |\__  \ | |
\     \____(  <_> )|   |  \ \___ \(  <_> )|  |__(  <_> )|   |  \|  | / __ \_\|
 \______  / \____/ |___|  //____  >\____/ |____/ \____/ |___|  /|__|(____  /__
        \/              \/      \/                           \/          \/ \/
```

## fonts:Consolonia#Cursive
Supported Font Sizes: 6
```
   __             _
  /  )           //            /
 /  ______ _  __//______ o__. /
(__(_/ / </_)(_</(_/ / <<(_/|'
                            o
```

## fonts:Consolonia#Doom
Supported Font Sizes: 8
```
 _____                       _             _       _
/  __ \                     | |           (_)     | |
| /  \/ ___  _ __  ___  ___ | | ___  _ __  _  __ _| |
| |    / _ \| '_ \/ __|/ _ \| |/ _ \| '_ \| |/ _` | |
| \__/| (_) | | | \__ | (_) | | (_) | | | | | (_| |_|
 \____/\___/|_| |_|___/\___/|_|\___/|_| |_|_|\__,_(_)
```

## fonts:Console#Mono
Supported Font Sizes: 8, 10
```

   ▄▄▄                              ▀▀█                    ▀             ▄
 ▄▀   ▀  ▄▄▄   ▄ ▄▄    ▄▄▄    ▄▄▄     █     ▄▄▄   ▄ ▄▄   ▄▄▄     ▄▄▄     █
 █      █▀ ▀█  █▀  █  █   ▀  █▀ ▀█    █    █▀ ▀█  █▀  █    █    ▀   █    █
 █      █   █  █   █   ▀▀▀▄  █   █    █    █   █  █   █    █    ▄▀▀▀█    ▀
  ▀▄▄▄▀ ▀█▄█▀  █   █  ▀▄▄▄▀  ▀█▄█▀    ▀▄▄  ▀█▄█▀  █   █  ▄▄█▄▄  ▀▄▄▀█    █

```

