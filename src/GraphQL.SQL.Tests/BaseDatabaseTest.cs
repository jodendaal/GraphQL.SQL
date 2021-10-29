using LocalDb;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace GraphQL.SQL.Tests
{
    [TestClass]
    public class BaseDatabaseTest
    {
        private static SqlInstance sqlInstance;
        public SqlDatabase Database { get; set; }
        static BaseDatabaseTest()
        {
            sqlInstance = new(
             name: "GraphQLSQLTest",
             buildTemplate: BaseDatabaseTest.CreateSchema);

        }

        public static async Task CreateSchema(DbConnection connection)
        {
            using (SqlConnection con = new SqlConnection(connection.ConnectionString))
            {
                var script = File.ReadAllText(@"Content\CreateDatabase.sql");
                Server server = new Server(new ServerConnection(con));
                server.ConnectionContext.ExecuteNonQuery(script);
            }
        }

        [TestInitialize]
        public async Task InitialiseDatabase()
        {
            Database = await sqlInstance.Build();
        }

        [TestCleanup]
        public void CleanUp()
        {
            sqlInstance.Cleanup();
        }
    }
}
