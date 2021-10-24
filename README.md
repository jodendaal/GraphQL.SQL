# GraphQL.SQL
Generate a GraphQL API from your SQL database in minutes.

## Features
Generates GraphQL Types rom database Schema
Advanced Set querying logic



## Container Registration

Use or extension method to quickly add the required services to [IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection):



<!-- snippet: RegisterInContainer -->
<a id='snippet-registerincontainer'></a>
```cs
 services.AddGraphQLSQL(_connectionString);
```
## App Registration

If you would like to use our middleware this can be added in Configure


<!-- snippet: RegisterInContainer -->
<a id='snippet-registerincontainer'></a>
```cs
  app.UseGraphQLLSqlApiMiddleWare();
```
