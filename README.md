# GraphQL.SQL for .NET
Generate a GraphQL API from your SQL database in minutes.

## Features
-   [x] Lightning fast and light weight
-   [x] Generates GraphQL Types from database Schema
-   [x] Generates Queries and Insert, Update and Delete Mutations
-   [x] Advanced Set querying logic
-   [x] Where conditions
-   [x] Order by
-   [x] One-To-Many joins
-   [x] Many-To-Many joins
-   [x] Paging
-   [x] Connections
-   [ ] Support for more databases
-   [ ] Documentation


## Container Registration

Use extension method to quickly add the required services to [IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection):



<!-- snippet: RegisterInContainer -->
<a id='snippet-registerincontainer'></a>
```cs
 services.AddGraphQLSQL(_connectionString);
```
## App Registration

If you would like to use the included middleware this can be added in Configure


<!-- snippet: RegisterInContainer -->
<a id='snippet-registerincontainer'></a>
```cs
  app.UseGraphQLLSqlApiMiddleWare();
```
