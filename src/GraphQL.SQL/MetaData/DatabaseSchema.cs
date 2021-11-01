using GraphQL.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphQL.SQL.MetaData
{
    public class DatabaseSchema
    {
        public List<Table> Tables { get; set; }
        public List<TableRelationShip> RelationShips { get; set; }
    }
}
