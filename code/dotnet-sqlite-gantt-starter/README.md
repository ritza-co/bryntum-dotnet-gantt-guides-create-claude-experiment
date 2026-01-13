
# Bryntum Gantt ASP.NET Core API starter

A basic ASP.NET Core API for performing CRUD operations on a SQLite database. For use with a Bryntum Gantt front end.

## Install dependencies

Run the following command to install the project dependencies:

```bash
dotnet restore
```

This installs Microsoft.AspNetCore.OpenApi, Microsoft.EntityFrameworkCore.Sqlite, and Microsoft.EntityFrameworkCore.Sqlite.

## Run the API server

Run the .NET server:

```bash
dotnet run
```

The server will start on `http://localhost:1337`.

There is an example API endpoint in `Program.cs`, `http://localhost:1337/api/hello` that returns "Hello World!".

## Install the .NET SDK using the C# Dev Kit VS Code extension

For a better developer experience, use VS Code and install the C# Dev Kit VS Code extension. Open VS Code and install the [C# Dev Kit VS Code extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit). This will also install the [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) and [.NET Install Tool](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscode-dotnet-runtime) extensions.

The C# Dev Kit VS Code extension will activate when you open a folder or workspace that contains a C# project such as this one. 

Do the first two get-started steps on the extension's welcome page:

- Connect your Microsoft account
- Install the .NET SDK