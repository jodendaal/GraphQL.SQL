using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.SQL.MetaData;
using GraphQL.Types;
using System.Data;
using System.Linq;
using System.Threading;

namespace GraphQL.SQL.Types
{
    public class TableFieldListRelationshipType : TableFieldType
    {
        public TableFieldListRelationshipType(TableObjectGraphType tableType, GraphQLTableMetaData childMetaData,TableRelationShip relationShip) : base(tableType)
        {
            Name = relationShip.FieldName;
            ResolvedType = new ListGraphType(base.ResolvedType);
            Resolver = new AsyncFieldResolver<object>(async context =>
               {
                   var loaderContext = context.RequestServices.GetService(typeof(IDataLoaderContextAccessor)) as IDataLoaderContextAccessor;
                   var database = context.RequestServices.GetService(typeof(IDatabase)) as IDatabase;

                   var loader = loaderContext.Context.GetOrAddBatchLoader<string, IGrouping<string, DataRow>>($"{childMetaData.NameAs}ById", (ids) =>
                   {
                       return database.GetRowsById(ids, CancellationToken.None, childMetaData, context);
                   });

                   if (context.Source != null)
                   {
                       var data = context.Source as IGrouping<string, DataRow>;
                       var row = data.First();
                       return loader.LoadAsync(row[childMetaData.IdentityColumn].ToString());
                   }
                   else
                   {
                       return loader.LoadAsync("1");//GetUsers from args
                   }
               });

            Metadata.Add("relationshipName", relationShip.FieldName);
        }
    }
}
