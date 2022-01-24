# ASP.NET Core API & Worker template

## What is this?

This is a template you can use to quickly create a multiplatform ASP.NET Core 6 application that runs an API server and a worker process as two projects under one solution. It also features fully configured packages that I use the most when developing ASP.NET apps, which are:

- [AutoMapper](https://github.com/AutoMapper/AutoMapper) - an object mapper with a [dependecy injection extension](https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection)
- [Dapper](https://github.com/DapperLib/Dapper) - a simple ORM mapper
- [Dapper.SimpleCRUD.yasikovsky](https://github.com/yasikovsky/Dapper.SimpleCRUD) - a fork of [Dapper.SimpleCRUD](https://github.com/ericdc1/Dapper.SimpleCRUD), a library for CRUD helpers for Dapper
- [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer) - for generating and serving JWT tokens
- [Npgsql](https://github.com/npgsql/npgsql) - a PostgreSQL data provider
- [Serilog](https://github.com/serilog/serilog) - a logging library with a bunch of extensions
-- **Serilog.AspNetCore** for dependency injection
-- **Serilog.Exceptions** for better exception handling
-- **Serilog.Sinks.Console** for native console logging
- [ServiceStack.Text](https://github.com/ServiceStack/ServiceStack.Text) - useful JSON manipulation library
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - for automagically generating Swagger API documentation

Also has a bunch of extension methods and useful methods I've written over the years and come back to all the time:

- **Custom Database Gateway** service for Database queries with dependency injection
- **Custom PostgreSQL Database resolver** for Dapper.SimpleCRUD 
- **Custom Dapper type handlers** for Enums (generic), List\<Guid>, List\<string>, List\<int>, RoleType and object (serialized to JSON)
- **Role** and **Claim** support with **RequireRole** and **RequireClaim** attributes that work with controllers and also generate Swagger annotations
- **FilterModel** and **SqlFilterAttribute** classes that help translate data request models with filters and sorting to database queries with joins across multiple tables
- solution-wide **appsettings.json** and **appsettings.[environment].json** to set-up config files once and share them between the API and Worker projects

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) for platform of your choice
- [ASP.NET Core 6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)  for the above

## Usage
1. Install the template nuget package: 
```bash
dotnet new --install yasikovsky.AspNetCoreTemplate
```
2. Navigate to your empty project folder
3. Create a project files using the template 

With example values: 
```bash
dotnet new yaspnet
```

Or with your own options:
```bash
dotnet new yaspnet --ProjectName "MyProject" --DbHostname "127.0.0.1" --DbPort "5432" --DbDatabase "mydatabase" --DbUsername "myusername" --DbPassword "mypassword" --GitHubOrgName "my-org-name"
```

4. Finish it up with restoring NuGet packages:
```bash
dotnet restore
```

## Changelog

1.0.0 
- Initial release

1.1.0
- Added Dockerfiles
- Added Preconfigured Docker-compose.yaml file with example environment variable file
- Added Github Actions runner to publish Docker packages to GitHub package repository when creating new release (important to tag them in the vX.X.X format)
- PasswordSalt is now a random UUID
- JwtAuthSecret is now two joined random UUIDs

1.1.1
- Small template modifications
