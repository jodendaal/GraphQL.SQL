using System;
using System.Collections.Generic;
using System.Data;

namespace GraphQL.SQL.Convertors
{
    public static class SqlDbTypeConvertorExtensions
    {
        private static Dictionary<string, SqlDbType>  _stringToSqlDbType = CreateSqlDbTypeMapping();

        private static Dictionary<string, SqlDbType> CreateSqlDbTypeMapping()
        {
            var typeMappings = new Dictionary<string, SqlDbType>(StringComparer.OrdinalIgnoreCase);

            foreach (SqlDbType t in Enum.GetValues(typeof(SqlDbType)))
            {
                typeMappings.Add(t.ToString().ToLowerInvariant(), t);
            }

            typeMappings.Add("numeric", SqlDbType.Decimal);

            return typeMappings;
        }

        public static SqlDbType ToSqlDbType(this string type)
        {
            if(_stringToSqlDbType.Count == 0)
            {
                CreateSqlDbTypeMapping();
            }

            if (_stringToSqlDbType.ContainsKey(type))
            {
                return _stringToSqlDbType[type];
            }
            
            throw new NotSupportedException($"{type} is not supported at present as a SqlDbType");
        }
    }
}
