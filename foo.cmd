erase /q packages\*.*
dotnet pack src/Consolonia.sln -c Release -o packages --include-symbols --property WarningLevel=0
