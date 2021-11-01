using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.SQL.Convertors;
using GraphQL.SQL.MetaData;
using GraphQL.Types;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.SQL.Types
{
    public class TableFieldType : FieldType
    {
        public TableFieldType(TableObjectGraphType tableType)
        {
            Name = tableType.SqlMetaData.NameAs;
            Type = tableType.GetType();
            Name = tableType.SqlMetaData.NameAs;
            Resolver = new AsyncFieldResolver<object>(async context =>
            {
                var loaderContext = context.RequestServices.GetService(typeof(IDataLoaderContextAccessor)) as IDataLoaderContextAccessor;
                var database = context.RequestServices.GetService(typeof(IDatabase)) as IDatabase;

                var loader = loaderContext.Context.GetOrAddBatchLoader<string, IGrouping<string, DataRow>>($"{tableType.SqlMetaData.NameAs}ById", (ids) =>
                {
                    return database.GetRowsById(ids, CancellationToken.None, tableType.SqlMetaData, context);
                });

                if (context.Source != null)
                {
                    var data = context.Source as IGrouping<string, DataRow>;
                    var row = data.First();
                    return loader.LoadAsync(row[tableType.SqlMetaData.IdentityColumn].ToString());
                }
                else
                {
                    return loader.LoadAsync("1");//GetUsers from args
                }
            });
            ResolvedType = tableType;
        }
    }
}
