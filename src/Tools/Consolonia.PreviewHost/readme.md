![logo](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/Logo.png)

# Consolonia.PreviewHost
This is a tool which enables a live view of a consolonia axaml file, or a consolonia project. 

![Preview Host](https://raw.githubusercontent.com/tomlm/ConsoloniaContent/main/PreviewHost.gif)

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
