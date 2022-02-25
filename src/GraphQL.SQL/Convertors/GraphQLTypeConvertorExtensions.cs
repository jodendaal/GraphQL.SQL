using GraphQL.DataLoader;
using GraphQL.SQL.MetaData;
using GraphQL.SQL.Types;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace GraphQL.SQL.Convertors
{
    public static class GraphQLTypeConvertorExtensions
    {
        private static Dictionary<string, Type> _sqlToGraphQLTypeMappings = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
        {
            { "varbinary", typeof(ByteGraphType) },
            { "binary", typeof(ByteGraphType) },
            { "timestamp", typeof(ByteGraphType) },
            { "image", typeof(ByteGraphType) },
            { "date", typeof(DateGraphType) },
            { "time", typeof(DateTimeGraphType) },
            { "datetime", typeof(DateTimeGraphType) },
            { "datetime2", typeof(DateTimeGraphType) },
            { "smalldatetime", typeof(DateTimeGraphType) },
            { "datetimeoffset", typeof(DateTimeGraphType) },
            { "bit", typeof(BooleanGraphType) },
            { "uniqueidentifier", typeof(GuidGraphType) },
            { "varchar", typeof(StringGraphType) },
            { "nchar", typeof(StringGraphType) },
            { "nvarchar", typeof(StringGraphType) },
            { "text", typeof(StringGraphType) },
            { "ntext", typeof(StringGraphType) },
            { "char", typeof(StringGraphType) },
            { "xml", typeof(StringGraphType) },
            { "float", typeof(FloatGraphType) },
            { "real", typeof(FloatGraphType) },
            { "decimal", typeof(DecimalGraphType) },
            { "numeric", typeof(DecimalGraphType) },
            { "money", typeof(DecimalGraphType) },
            { "smallmoney", typeof(DecimalGraphType) },
            { "bigint", typeof(BigIntGraphType) },
            { "int", typeof(IntGraphType) },
            { "tinyint", typeof(IntGraphType) },
            { "smallint", typeof(IntGraphType) }
        };

        public static Type ToGraphQLType(this string dbType)
        {
            if (_sqlToGraphQLTypeMappings.ContainsKey(dbType))
            {
                return _sqlToGraphQLTypeMappings[dbType];
            }

            throw new NotSupportedException($"{dbType} is not supported at present as a GraphQL Type");
        }

        public static TableObjectGraphType ToObjectGraph(this GraphQLTableMetaData table)
        {
            return new TableObjectGraphType(table);
        }

        public static Dictionary<string, TableObjectGraphType> ToGraphQLTypes(this GraphQLSqlMetaData metaData)
        {
            var result = new Dictionary<string, TableObjectGraphType>(StringComparer.OrdinalIgnoreCase);
            foreach (var table in metaData.Tables)
            {
                var tableType = table.ToObjectGraph();
                result.Add(tableType.Name, tableType);
            }

            // relationShips
            foreach (var relationShip in metaData.RelationShips)
            {
                var parentTable = metaData.Tables.FirstOrDefault(i => i.TableName.ToLowerInvariant() == relationShip.ParentTable.ToLowerInvariant());
                var childTable = metaData.Tables.FirstOrDefault(i => i.TableName.ToLowerInvariant() == relationShip.ChildTable.ToLowerInvariant());
                if (!result.ContainsKey(parentTable.NameAs))
                {
                    break;
                }

                var parent = result[parentTable.NameAs];
                var child = result[childTable.NameAs];

                if (relationShip.IsList)
                {
                    var field = new TableFieldListRelationshipType(result[childTable.NameAs], childTable, relationShip);
                    parent.AddField(field);
                }
                else
                {
                    var field = new TableFieldType(result[childTable.NameAs]) { Name = relationShip.FieldName };
                    field.Metadata.Add("relationshipName", relationShip.FieldName);
                    parent.AddField(field);
                }
            }

            return result;
        }
    }
}
