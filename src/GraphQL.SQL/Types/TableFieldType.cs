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
            Resolver = new TableFieldTypeResolver(tableType.SqlMetaData);
            ResolvedType = tableType;
        }
    }
}
