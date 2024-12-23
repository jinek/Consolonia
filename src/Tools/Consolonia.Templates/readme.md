# Iciclecreek Bot Templates
This is a package which adds new templates to dotnet tempalte libraries. 

## Installation
```
dotnet new -i Iciclecreek.Bot.Templates
```

## Bot Templates
Templates for creating bots.

### ConsoleBot template
Creates a Console bot which uses Lucy Recognizer to create natural language console app.

```
dotnet new consolebot --name {botName}
```


### LucyBot Template
Creates a Azure Function bot with Lucy Recognizer 

```
dotnet new lucybot --name {botName}
```

## Dialog Item Templates
Item templates for creating dialogs.

### LucyDialog Template
Creates a new code first dialog configured with LucyRecognizer.

```
cd Dialogs
dotnet new lucydialog --name {dialogname}
cd ..
```
