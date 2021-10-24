# GraphQL.SQL
Generate a GraphQL API from your SQL database in minutes.

## Container Registration

Enabling is done via registering in a container.

The container registration can be done via adding to a [IServiceCollection](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection):

<!-- snippet: RegisterInContainer -->
<a id='snippet-registerincontainer'></a>
```cs
 services.AddGraphQLSQL(_connectionString);
```
