Przed komplilacją aplikacji wprowadź w Package Manager Console:

```cmd
dotnet nuget remove source "Microsoft Visual Studio Offline Packages"
dotnet nuget add source https://api.nuget.org/v3/index.json --name nuget.org
Update-Database
```
