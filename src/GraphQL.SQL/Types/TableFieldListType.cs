using GraphQL.Resolvers;
using GraphQL.Types;
using System.Collections;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.SQL.Types
{
    public class TableFieldListType : TableFieldType
    {
        public TableFieldListType(TableObjectGraphType tableType,IFieldResolver fieldResolver = null) : base(tableType)
        {
            var metaData = tableType.SqlMetaData;
            Name = metaData.NameAs;
            ResolvedType = new ListGraphType(base.ResolvedType);
            Resolver = fieldResolver == null? new AsyncFieldResolver<ILookup<string, DataRow>>(async context =>
            {
                var database = context.RequestServices.GetService(typeof(IDatabase)) as IDatabase;
                var data = await database.Query(CancellationToken.None, metaData, context);
                return data;
            }) : fieldResolver;
            Arguments = new TableQueryArgsType(metaData);
        }
    }
}
