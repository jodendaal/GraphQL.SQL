using System.Collections.Generic;

namespace GraphQL.SQL.MetaData
{
    public class GraphQLSqlMetaData
    {
        public List<GraphQLTableMetaData> Tables { get; set; }

        public List<TableRelationShip> RelationShips { get; set; }
    }
}
