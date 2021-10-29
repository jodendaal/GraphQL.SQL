using LocalDb;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests
{
    [TestClass]
    public class BaseDatabaseTest
    {
        public static SqlDatabase Database;
        private static SqlInstance sqlInstance;
        static BaseDatabaseTest()
        {
            sqlInstance = new(
             name: "GraphQLSQLTestNew",
             buildTemplate: CreateSchema
             );
        }

        public static async Task CreateSchema(DbConnection connection)
        {
            using (SqlConnection con = new SqlConnection(connection.ConnectionString))
            {
                var script = File.ReadAllText(@"Content\CreateDatabase.sql");
                Server server = new Server(new ServerConnection(con));
                server.ConnectionContext.ExecuteNonQuery(script);
            }
           
            await Task.Delay(0);
        }


        public async Task<SqlDatabase> GetDatabase([CallerMemberName] string id = "")
        {
            if(Database == null)
            {
                Database = await sqlInstance.Build(memberName: id);
            }
            return Database;
        }

        [ClassCleanup]
        public void CleanUp()
        {
            try
            {
                Debug.Print("CLeaning up databases");
                
                sqlInstance.Cleanup();
            }
            catch { }
        }

        ~BaseDatabaseTest()
        {
            try
            {
                Debug.Print("CLeaning up databases");
                sqlInstance.Cleanup();
            }
            catch { }
        }
    }
}
