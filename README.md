Release new version:

In the project folder:
in `cslox.csproj` file, update version number
run `dotnet pack` to create a new package
run `dotnet tool update -g --add-source ./nupkg cslox` to update the global tool