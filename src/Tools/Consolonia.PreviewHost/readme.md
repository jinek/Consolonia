# Consolonia.PreviewHost
This is a tool which enables a live view of a consolonia axaml file, or a consolonia project. 

## Background
Consolonia is a TUI (Text User Interface) (GUI Framework) implementation for [Avalonia UI](https://github.com/AvaloniaUI)

Supports XAML, data bindings, animation, styling and the rest from Avalonia.

## Showcase (click picture to see video)
[![datagridpic](https://user-images.githubusercontent.com/10516222/141980173-4eb4057a-6996-45bf-83f6-931316c98d88.png)](https://youtu.be/ttgZmbruk3Y)

## Installation
To install the tool, you can use the following command:
```
dotnet tool install -g Consolonia.PreviewHost
```

## Usage
To use the tool, you can use the following command:
```
Consolonia.PreviewHost <path-to-axaml-file>|<path-to-project-file>|<path-to-project-directory>
```

It will monitor the file or directory for changes, and will automatically update the preview when changes are detected.

Run the following command to monitor the project or folder:

``` Conoslonia.PreviewHost <path-to-project-file-or-folder> ```

Run following command to monitor a given .axaml file. 

``` Conoslonia.PreviewHost <.axaml file name> ```

> NOTE: In single file mode the tool will exit when you hit escape key

### Running inside Visual Studio
1. Open the solution in Visual Studio.
1. Open a Terminal (View -> Terminal).
3. run the following command
``` Consolonia.PreviewHost . ```

### Running inside Visual Studio Code
To integrate the tool with Visual Studio Code, you can use the following steps:
1. Open the project directory in Visual Studio Code.
2. Open a terminal inside of visual studio code
3. run the following command
``` Consolonia.PreviewHost . ```
