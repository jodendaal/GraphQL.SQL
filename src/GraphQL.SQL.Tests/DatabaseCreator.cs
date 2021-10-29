using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests
{
    public class TestDatabase
    {
        public static async Task CreateSchema(DbConnection connection)
        {
            using (SqlConnection con = new SqlConnection(connection.ConnectionString))
            {
                var script = File.ReadAllText(@"Content\CreateDatabase.sql");
                Server server = new Server(new ServerConnection(con));
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }
    }
}
