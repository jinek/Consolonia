# Contributing Guide

## Preparation
To compile and debug the application it's necessary to install [dotnet 5 sdk](https://dotnet.microsoft.com/en-us/download/dotnet/6.0). It's possible to use any compliant tools for coding and debugging.

## Tickets And Discussions
Feel free to open a [ticket](https://github.com/jinek/Consolonia/issues) or a [discussion](https://github.com/jinek/Consolonia/discussions) in case you have issues, need help, or just have some ideas.

In case of a bug, first try to check whether issue exist in [AvaloniaUI](https://github.com/AvaloniaUI/Avalonia) - Avalonia community also accepts tickets, we should consider fixing it in Avalonia rather than in Consolonia if applicable.

## Code Style
Projects are setup to utilize [dotnet analyzers](https://docs.microsoft.com/en-us/dotnet/framework/code-analyzers).
All warning are considered as errors (for `RELEASE` configuration only), thus, check there are no warnings when compiling `DEBUG` configuration.
Formatting and styling is set by editorconfig.yml which can be recognized by [JetBrains Resharper or Rider](https://www.jetbrains.com/resharper/). If you don't have one, just make a pull request and check build statuses. It does automatic formatting and checks the style.
Also feel free to propose additional analyzers or changes to existing ones.

## Pull Request
When preparing a pull request, please, ensure your commits have description of the changes. Eventually the PR can be merged only by squashing the commits, thus final message will be a concatenation of the commit messages.
