# GraphQL.SQL
Generate a GraphQL API from your SQL database in minutes.

## Container Registration

Use or extension method to quickly register the services

The container registration can be done via adding to a [IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection):

<!-- snippet: RegisterInContainer -->
<a id='snippet-registerincontainer'></a>
```cs
 services.AddGraphQLSQL(_connectionString);
```
