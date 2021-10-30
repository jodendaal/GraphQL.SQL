using GraphQL.Types;
using System;
using System.Collections.Generic;

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
    }
}
