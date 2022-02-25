using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.SQL.Convertors;
using GraphQL.SQL.MetaData;
using GraphQL.SQL.Types;
using GraphQL.Types;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.SQL
{
    public partial class SqlQuery : ObjectGraphType
    {
        public SqlQuery(IMetaDataProvider metaDataProvider)
        {
            var gqlMetaData = metaDataProvider.GetMetaData();
            var types = gqlMetaData.ToGraphQLTypes();

            foreach (var tableType in types)
            {
                AddField(new TableFieldListType(tableType.Value));
            }
        }
    }
}
