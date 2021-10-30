using GraphQL.SQL.Convertors;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests.Convertors
{
    [TestClass]
    public class SqlDbTypeConvertorExtensions : BaseDatabaseTest
    {
        //Will look into these in future version
        List<string> _unsupportedTypes = new(){ "geography", "geometry", "hierarchyid", "sql_variant", "sysname" };

        private async Task<List<string>> GetSqlDbTypes()
        {
            var types = new List<string>();
            var database = await GetDatabase();
            using (var cmd = new SqlCommand("SELECT name as [Type] FROM sys.types", database.Connection))
            {
                using(var rdr = await cmd.ExecuteReaderAsync())
                {
                    while (rdr.Read())
                    {
                        types.Add(rdr["Type"].ToString());
                    }
                }
            }
            return types;
        }

        [TestMethod]
        public async Task StringToDbType()
        {
            
            var sqlTypes = await GetSqlDbTypes();

            foreach (var sqlType in sqlTypes)
            {
                if (!_unsupportedTypes.Contains(sqlType))
                {
                    var sqlDbType = sqlType.ToSqlDbType();
                }
            }
        }

        [TestMethod]
        public async Task DbTypeToGraphQLType()
        {
            //Will look into these in future version
           
            var sqlTypes = await GetSqlDbTypes();

            foreach (var sqlType in sqlTypes)
            {
                if (!_unsupportedTypes.Contains(sqlType))
                {
                    var graphQLType = sqlType.ToGraphQLType();
                }
            }
        }
    }
}
