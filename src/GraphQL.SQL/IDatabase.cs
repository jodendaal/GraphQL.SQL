using GraphQL.SQL.MetaData;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GraphQL.SQL
{
    public interface IDatabase
    {
        Task<IDictionary<string, IGrouping<string, DataRow>>> GetRowsById(IEnumerable<string> ids, CancellationToken cancellationToken, GraphQLTableMetaData metaData, IResolveFieldContext context);
        Task<ILookup<string, DataRow>> Query(CancellationToken cancellationToken, GraphQLTableMetaData metaData, IResolveFieldContext context);
    }
}