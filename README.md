# cslox
cslox, a C# Lox implementation

## Installation
To install the tool, run the following command:
```bash
dotnet pack
dotnet tool install -g  --add-source ./nupkg cslox
```

## ﻿Updating the package

In the project folder:

In `cslox.csproj` file, update version number.

To update the package, run the following command:
```bash
dotnet pack
dotnet tool update -g --add-source ./nupkg cslox
```

## Usage
After installing the version globally you can use it anywhere in a command line.

To run an interactive shell, run the following command:
```bash
cslox
```

To run a script file, run the following command:
```bash
cslox <script>
```