using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.SQL.MetaData;
using System.Data;
using System.Linq;
using System.Threading;

namespace GraphQL.SQL.Types
{
    public class TableFieldTypeResolver : IFieldResolver
    {
        private readonly GraphQLTableMetaData _tableMetaData;

        public TableFieldTypeResolver(GraphQLTableMetaData tableMetaData)
        {
            _tableMetaData = tableMetaData;
        }

        public object Resolve(IResolveFieldContext context)
        {
            var loaderContext = context.RequestServices.GetService(typeof(IDataLoaderContextAccessor)) as IDataLoaderContextAccessor;
            var database = context.RequestServices.GetService(typeof(IDatabase)) as IDatabase;

            var loader = loaderContext.Context.GetOrAddBatchLoader<string, IGrouping<string, DataRow>>($"{_tableMetaData.NameAs}ById", (ids) =>
            {
                return database.GetRowsById(ids, CancellationToken.None, _tableMetaData, context);
            });

            if (context.Source != null)
            {
                var data = context.Source as IGrouping<string, DataRow>;
                var row = data.First();
                return loader.LoadAsync(row[_tableMetaData.IdentityColumn].ToString());
            }
            else
            {
                return loader.LoadAsync("1");//GetUsers from args
            }
        }
    }
}
